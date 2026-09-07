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
    public interface IFornecedorService
    {
        Task<FornecedorRespostaDto> Criar(FornecedorCriacaoDto dto);
        Task<FornecedorRespostaDto> Atualizar(int id, FornecedorAtualizacaoDto dto);
        Task<FornecedorRespostaDto> ObterPorId(int id);
        Task<List<FornecedorRespostaDto>> Listar(string texto, bool? ativo);
        Task<FornecedorRespostaDto> AlterarSituacao(int id, bool ativo);
    }

    public class FornecedorService : IFornecedorService
    {
        private readonly AppDbContext _contexto;

        public FornecedorService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<FornecedorRespostaDto> Criar(FornecedorCriacaoDto dto)
        {
            var cnpjExiste = await _contexto.Fornecedores.AnyAsync(f => f.Cnpj == dto.Cnpj);
            if (cnpjExiste)
            {
                throw new RegraNegocioException("Ja existe um fornecedor com este CNPJ.");
            }

            var fornecedor = new Fornecedor
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Ativo = true
            };

            _contexto.Fornecedores.Add(fornecedor);
            await _contexto.SaveChangesAsync();

            return Converter(fornecedor);
        }

        public async Task<FornecedorRespostaDto> Atualizar(int id, FornecedorAtualizacaoDto dto)
        {
            var fornecedor = await _contexto.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                throw new NaoEncontradoException("Fornecedor nao encontrado.");
            }

            var cnpjEmUso = await _contexto.Fornecedores.AnyAsync(f => f.Cnpj == dto.Cnpj && f.Id != id);
            if (cnpjEmUso)
            {
                throw new RegraNegocioException("Ja existe outro fornecedor com este CNPJ.");
            }

            fornecedor.Nome = dto.Nome;
            fornecedor.Cnpj = dto.Cnpj;
            fornecedor.Email = dto.Email;
            fornecedor.Telefone = dto.Telefone;

            await _contexto.SaveChangesAsync();

            return Converter(fornecedor);
        }

        public async Task<FornecedorRespostaDto> ObterPorId(int id)
        {
            var fornecedor = await _contexto.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                throw new NaoEncontradoException("Fornecedor nao encontrado.");
            }

            return Converter(fornecedor);
        }

        public async Task<List<FornecedorRespostaDto>> Listar(string texto, bool? ativo)
        {
            var consulta = _contexto.Fornecedores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var termo = texto.Trim().ToLower();
                consulta = consulta.Where(f => f.Nome.ToLower().Contains(termo) || f.Cnpj.Contains(termo));
            }

            if (ativo.HasValue)
            {
                consulta = consulta.Where(f => f.Ativo == ativo.Value);
            }

            var fornecedores = await consulta.OrderBy(f => f.Nome).ToListAsync();
            return fornecedores.Select(Converter).ToList();
        }

        public async Task<FornecedorRespostaDto> AlterarSituacao(int id, bool ativo)
        {
            var fornecedor = await _contexto.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                throw new NaoEncontradoException("Fornecedor nao encontrado.");
            }

            fornecedor.Ativo = ativo;
            await _contexto.SaveChangesAsync();

            return Converter(fornecedor);
        }

        private static FornecedorRespostaDto Converter(Fornecedor fornecedor)
        {
            return new FornecedorRespostaDto
            {
                Id = fornecedor.Id,
                Nome = fornecedor.Nome,
                Cnpj = fornecedor.Cnpj,
                Email = fornecedor.Email,
                Telefone = fornecedor.Telefone,
                Ativo = fornecedor.Ativo
            };
        }
    }
}
