using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services
{
    public interface IUnidadeService
    {
        Task<UnidadeRespostaDto> Criar(UnidadeCriacaoDto dto);
        Task<UnidadeRespostaDto> Atualizar(int id, UnidadeAtualizacaoDto dto);
        Task<UnidadeRespostaDto> ObterPorId(int id);
        Task<List<UnidadeRespostaDto>> Listar(bool? ativa, string texto);
        Task<UnidadeRespostaDto> AlterarSituacao(int id, bool ativa);
        Task<ResponsavelRespostaDto> AdicionarResponsavel(int unidadeId, ResponsavelDto dto);
    }

    public class UnidadeService : IUnidadeService
    {
        private readonly AppDbContext _contexto;

        public UnidadeService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<UnidadeRespostaDto> Criar(UnidadeCriacaoDto dto)
        {
            var franqueadoraExiste = await _contexto.Franqueadoras.AnyAsync(f => f.Id == dto.FranqueadoraId);
            if (!franqueadoraExiste)
            {
                throw new NaoEncontradoException("Franqueadora nao encontrada.");
            }

            var cnpjExiste = await _contexto.Unidades.AnyAsync(u => u.Cnpj == dto.Cnpj);
            if (cnpjExiste)
            {
                throw new RegraNegocioException("Ja existe uma unidade cadastrada com este CNPJ.");
            }

            var unidade = new UnidadeFranqueada
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Endereco = dto.Endereco,
                Telefone = dto.Telefone,
                Email = dto.Email,
                DataInicio = dto.DataInicio,
                PercentualRoyalty = dto.PercentualRoyalty,
                Ativa = true,
                FranqueadoraId = dto.FranqueadoraId
            };

            var responsaveis = dto.Responsaveis ?? new List<ResponsavelDto>();
            foreach (var responsavel in responsaveis)
            {
                unidade.Responsaveis.Add(new Responsavel
                {
                    Nome = responsavel.Nome,
                    Cpf = responsavel.Cpf,
                    Email = responsavel.Email,
                    Telefone = responsavel.Telefone,
                    Franqueado = responsavel.Franqueado
                });
            }

            _contexto.Unidades.Add(unidade);
            await _contexto.SaveChangesAsync();

            return Converter(unidade);
        }

        public async Task<UnidadeRespostaDto> Atualizar(int id, UnidadeAtualizacaoDto dto)
        {
            var unidade = await _contexto.Unidades
                .Include(u => u.Responsaveis)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            var cnpjEmUso = await _contexto.Unidades.AnyAsync(u => u.Cnpj == dto.Cnpj && u.Id != id);
            if (cnpjEmUso)
            {
                throw new RegraNegocioException("Ja existe outra unidade com este CNPJ.");
            }

            unidade.Nome = dto.Nome;
            unidade.Cnpj = dto.Cnpj;
            unidade.Cidade = dto.Cidade;
            unidade.Estado = dto.Estado;
            unidade.Endereco = dto.Endereco;
            unidade.Telefone = dto.Telefone;
            unidade.Email = dto.Email;
            unidade.DataInicio = dto.DataInicio;
            unidade.PercentualRoyalty = dto.PercentualRoyalty;

            await _contexto.SaveChangesAsync();

            return Converter(unidade);
        }

        public async Task<UnidadeRespostaDto> ObterPorId(int id)
        {
            var unidade = await _contexto.Unidades
                .Include(u => u.Responsaveis)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            return Converter(unidade);
        }

        public async Task<List<UnidadeRespostaDto>> Listar(bool? ativa, string texto)
        {
            var consulta = _contexto.Unidades
                .Include(u => u.Responsaveis)
                .AsQueryable();

            if (ativa.HasValue)
            {
                consulta = consulta.Where(u => u.Ativa == ativa.Value);
            }

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var termo = texto.Trim().ToLower();
                consulta = consulta.Where(u =>
                    u.Nome.ToLower().Contains(termo) ||
                    u.Cidade.ToLower().Contains(termo) ||
                    u.Cnpj.Contains(termo) ||
                    u.Responsaveis.Any(r => r.Nome.ToLower().Contains(termo)));
            }

            var unidades = await consulta.OrderBy(u => u.Nome).ToListAsync();
            return unidades.Select(Converter).ToList();
        }

        public async Task<UnidadeRespostaDto> AlterarSituacao(int id, bool ativa)
        {
            var unidade = await _contexto.Unidades
                .Include(u => u.Responsaveis)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            unidade.Ativa = ativa;
            await _contexto.SaveChangesAsync();

            return Converter(unidade);
        }

        public async Task<ResponsavelRespostaDto> AdicionarResponsavel(int unidadeId, ResponsavelDto dto)
        {
            var unidade = await _contexto.Unidades.FindAsync(unidadeId);
            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            var responsavel = new Responsavel
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Franqueado = dto.Franqueado,
                UnidadeFranqueadaId = unidadeId
            };

            _contexto.Responsaveis.Add(responsavel);
            await _contexto.SaveChangesAsync();

            return new ResponsavelRespostaDto
            {
                Id = responsavel.Id,
                Nome = responsavel.Nome,
                Cpf = responsavel.Cpf,
                Email = responsavel.Email,
                Telefone = responsavel.Telefone,
                Franqueado = responsavel.Franqueado
            };
        }

        private static UnidadeRespostaDto Converter(UnidadeFranqueada unidade)
        {
            return new UnidadeRespostaDto
            {
                Id = unidade.Id,
                Nome = unidade.Nome,
                Cnpj = unidade.Cnpj,
                Cidade = unidade.Cidade,
                Estado = unidade.Estado,
                Endereco = unidade.Endereco,
                Telefone = unidade.Telefone,
                Email = unidade.Email,
                DataInicio = unidade.DataInicio,
                Ativa = unidade.Ativa,
                PercentualRoyalty = unidade.PercentualRoyalty,
                FranqueadoraId = unidade.FranqueadoraId,
                Responsaveis = unidade.Responsaveis
                    .Select(r => new ResponsavelRespostaDto
                    {
                        Id = r.Id,
                        Nome = r.Nome,
                        Cpf = r.Cpf,
                        Email = r.Email,
                        Telefone = r.Telefone,
                        Franqueado = r.Franqueado
                    })
                    .ToList()
            };
        }
    }
}
