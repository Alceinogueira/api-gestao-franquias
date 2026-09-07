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
    public interface IChamadoService
    {
        Task<ChamadoRespostaDto> Criar(ChamadoCriacaoDto dto);
        Task<ChamadoRespostaDto> Atualizar(int id, ChamadoAtualizacaoDto dto);
        Task<ChamadoRespostaDto> ObterPorId(int id);
        Task<List<ChamadoRespostaDto>> Listar(int? unidadeId, StatusChamado? status, PrioridadeChamado? prioridade);
    }

    public class ChamadoService : IChamadoService
    {
        private readonly AppDbContext _contexto;

        public ChamadoService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<ChamadoRespostaDto> Criar(ChamadoCriacaoDto dto)
        {
            var unidade = await _contexto.Unidades.FindAsync(dto.UnidadeFranqueadaId);
            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            var chamado = new ChamadoSuporte
            {
                UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
                Titulo = dto.Titulo,
                Categoria = dto.Categoria,
                Descricao = dto.Descricao,
                Prioridade = dto.Prioridade,
                Status = StatusChamado.Aberto,
                DataAbertura = DateTime.UtcNow
            };

            _contexto.Chamados.Add(chamado);
            await _contexto.SaveChangesAsync();

            return await ObterPorId(chamado.Id);
        }

        public async Task<ChamadoRespostaDto> Atualizar(int id, ChamadoAtualizacaoDto dto)
        {
            var chamado = await _contexto.Chamados.FindAsync(id);
            if (chamado == null)
            {
                throw new NaoEncontradoException("Chamado nao encontrado.");
            }

            if (chamado.Status == StatusChamado.Encerrado)
            {
                throw new RegraNegocioException("Chamado encerrado nao pode ser alterado.");
            }

            chamado.Status = dto.Status;
            chamado.Resolucao = dto.Resolucao;

            if (dto.Status == StatusChamado.Encerrado)
            {
                chamado.DataEncerramento = DateTime.UtcNow;
            }

            await _contexto.SaveChangesAsync();

            return await ObterPorId(chamado.Id);
        }

        public async Task<ChamadoRespostaDto> ObterPorId(int id)
        {
            var chamado = await _contexto.Chamados
                .Include(c => c.UnidadeFranqueada)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chamado == null)
            {
                throw new NaoEncontradoException("Chamado nao encontrado.");
            }

            return Converter(chamado);
        }

        public async Task<List<ChamadoRespostaDto>> Listar(int? unidadeId, StatusChamado? status, PrioridadeChamado? prioridade)
        {
            var consulta = _contexto.Chamados
                .Include(c => c.UnidadeFranqueada)
                .AsQueryable();

            if (unidadeId.HasValue)
            {
                consulta = consulta.Where(c => c.UnidadeFranqueadaId == unidadeId.Value);
            }

            if (status.HasValue)
            {
                consulta = consulta.Where(c => c.Status == status.Value);
            }

            if (prioridade.HasValue)
            {
                consulta = consulta.Where(c => c.Prioridade == prioridade.Value);
            }

            var chamados = await consulta
                .OrderByDescending(c => c.Prioridade)
                .ThenByDescending(c => c.DataAbertura)
                .ToListAsync();

            return chamados.Select(Converter).ToList();
        }

        private static ChamadoRespostaDto Converter(ChamadoSuporte chamado)
        {
            return new ChamadoRespostaDto
            {
                Id = chamado.Id,
                UnidadeFranqueadaId = chamado.UnidadeFranqueadaId,
                Unidade = chamado.UnidadeFranqueada != null ? chamado.UnidadeFranqueada.Nome : null,
                Titulo = chamado.Titulo,
                Categoria = chamado.Categoria,
                Descricao = chamado.Descricao,
                Prioridade = chamado.Prioridade.ToString(),
                Status = chamado.Status.ToString(),
                DataAbertura = chamado.DataAbertura,
                DataEncerramento = chamado.DataEncerramento,
                Resolucao = chamado.Resolucao
            };
        }
    }
}
