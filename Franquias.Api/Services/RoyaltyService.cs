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
    public interface IRoyaltyService
    {
        Task<RoyaltyRespostaDto> Gerar(GerarRoyaltyDto dto);
        Task<RoyaltyRespostaDto> RegistrarPagamento(int id);
        Task<RoyaltyRespostaDto> ObterPorId(int id);
        Task<List<RoyaltyRespostaDto>> Listar(int? unidadeId, int? ano, int? mes, StatusPagamento? status);
    }

    public class RoyaltyService : IRoyaltyService
    {
        private readonly AppDbContext _contexto;

        public RoyaltyService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<RoyaltyRespostaDto> Gerar(GerarRoyaltyDto dto)
        {
            var unidade = await _contexto.Unidades.FindAsync(dto.UnidadeFranqueadaId);
            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            var jaGerado = await _contexto.Royalties.AnyAsync(r =>
                r.UnidadeFranqueadaId == dto.UnidadeFranqueadaId && r.Ano == dto.Ano && r.Mes == dto.Mes);
            if (jaGerado)
            {
                throw new RegraNegocioException("O royalty deste periodo ja foi gerado para esta unidade.");
            }

            var inicio = new DateTime(dto.Ano, dto.Mes, 1);
            var fim = inicio.AddMonths(1);

            var valoresVendas = await _contexto.Vendas
                .Where(v => v.UnidadeFranqueadaId == dto.UnidadeFranqueadaId
                    && v.Status == StatusVenda.Confirmada
                    && v.Data >= inicio && v.Data < fim)
                .Select(v => v.ValorTotal)
                .ToListAsync();

            var faturamento = valoresVendas.Sum();
            var percentual = unidade.PercentualRoyalty;
            var valorDevido = Math.Round(faturamento * percentual / 100m, 2);

            var royalty = new Royalty
            {
                UnidadeFranqueadaId = unidade.Id,
                Ano = dto.Ano,
                Mes = dto.Mes,
                FaturamentoPeriodo = faturamento,
                PercentualAplicado = percentual,
                ValorDevido = valorDevido,
                StatusPagamento = StatusPagamento.Pendente,
                DataGeracao = DateTime.UtcNow
            };

            _contexto.Royalties.Add(royalty);
            await _contexto.SaveChangesAsync();

            return await ObterPorId(royalty.Id);
        }

        public async Task<RoyaltyRespostaDto> RegistrarPagamento(int id)
        {
            var royalty = await _contexto.Royalties.FindAsync(id);
            if (royalty == null)
            {
                throw new NaoEncontradoException("Royalty nao encontrado.");
            }

            if (royalty.StatusPagamento == StatusPagamento.Pago)
            {
                throw new RegraNegocioException("Este royalty ja esta pago.");
            }

            royalty.StatusPagamento = StatusPagamento.Pago;
            royalty.DataPagamento = DateTime.UtcNow;
            await _contexto.SaveChangesAsync();

            return await ObterPorId(royalty.Id);
        }

        public async Task<RoyaltyRespostaDto> ObterPorId(int id)
        {
            var royalty = await _contexto.Royalties
                .Include(r => r.UnidadeFranqueada)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (royalty == null)
            {
                throw new NaoEncontradoException("Royalty nao encontrado.");
            }

            return Converter(royalty);
        }

        public async Task<List<RoyaltyRespostaDto>> Listar(int? unidadeId, int? ano, int? mes, StatusPagamento? status)
        {
            var consulta = _contexto.Royalties
                .Include(r => r.UnidadeFranqueada)
                .AsQueryable();

            if (unidadeId.HasValue)
            {
                consulta = consulta.Where(r => r.UnidadeFranqueadaId == unidadeId.Value);
            }

            if (ano.HasValue)
            {
                consulta = consulta.Where(r => r.Ano == ano.Value);
            }

            if (mes.HasValue)
            {
                consulta = consulta.Where(r => r.Mes == mes.Value);
            }

            if (status.HasValue)
            {
                consulta = consulta.Where(r => r.StatusPagamento == status.Value);
            }

            var royalties = await consulta
                .OrderByDescending(r => r.Ano)
                .ThenByDescending(r => r.Mes)
                .ToListAsync();

            return royalties.Select(Converter).ToList();
        }

        private static RoyaltyRespostaDto Converter(Royalty royalty)
        {
            return new RoyaltyRespostaDto
            {
                Id = royalty.Id,
                UnidadeFranqueadaId = royalty.UnidadeFranqueadaId,
                Unidade = royalty.UnidadeFranqueada != null ? royalty.UnidadeFranqueada.Nome : null,
                Ano = royalty.Ano,
                Mes = royalty.Mes,
                FaturamentoPeriodo = royalty.FaturamentoPeriodo,
                PercentualAplicado = royalty.PercentualAplicado,
                ValorDevido = royalty.ValorDevido,
                StatusPagamento = royalty.StatusPagamento.ToString(),
                DataGeracao = royalty.DataGeracao,
                DataPagamento = royalty.DataPagamento
            };
        }
    }
}
