using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services
{
    public interface IRelatorioService
    {
        Task<List<FaturamentoUnidadeDto>> FaturamentoPorUnidade(DateTime inicio, DateTime fim);
        Task<List<RankingUnidadeDto>> RankingUnidades(DateTime inicio, DateTime fim);
        Task<List<ProdutoMaisVendidoDto>> ProdutosMaisVendidos(DateTime? inicio, DateTime? fim, int top);
        Task<TotalRoyaltiesDto> TotalRoyalties(int? ano, int? mes);
        Task<List<EstoqueRespostaDto>> EstoqueCritico();
        Task<List<ChamadosPorStatusDto>> ChamadosPorStatus();
    }

    public class RelatorioService : IRelatorioService
    {
        private readonly AppDbContext _contexto;
        private readonly IEstoqueService _estoqueService;

        public RelatorioService(AppDbContext contexto, IEstoqueService estoqueService)
        {
            _contexto = contexto;
            _estoqueService = estoqueService;
        }

        public async Task<List<FaturamentoUnidadeDto>> FaturamentoPorUnidade(DateTime inicio, DateTime fim)
        {
            var limite = fim.Date.AddDays(1);

            var vendas = await _contexto.Vendas
                .Include(v => v.UnidadeFranqueada)
                .Where(v => v.Status == StatusVenda.Confirmada && v.Data >= inicio && v.Data < limite)
                .ToListAsync();

            return vendas
                .GroupBy(v => new { v.UnidadeFranqueadaId, Nome = v.UnidadeFranqueada.Nome })
                .Select(g => new FaturamentoUnidadeDto
                {
                    UnidadeFranqueadaId = g.Key.UnidadeFranqueadaId,
                    Unidade = g.Key.Nome,
                    QuantidadeVendas = g.Count(),
                    Faturamento = g.Sum(v => v.ValorTotal)
                })
                .OrderByDescending(x => x.Faturamento)
                .ToList();
        }

        public async Task<List<RankingUnidadeDto>> RankingUnidades(DateTime inicio, DateTime fim)
        {
            var faturamentos = await FaturamentoPorUnidade(inicio, fim);

            var ranking = new List<RankingUnidadeDto>();
            var posicao = 1;
            foreach (var item in faturamentos)
            {
                ranking.Add(new RankingUnidadeDto
                {
                    Posicao = posicao,
                    UnidadeFranqueadaId = item.UnidadeFranqueadaId,
                    Unidade = item.Unidade,
                    Faturamento = item.Faturamento
                });
                posicao++;
            }

            return ranking;
        }

        public async Task<List<ProdutoMaisVendidoDto>> ProdutosMaisVendidos(DateTime? inicio, DateTime? fim, int top)
        {
            if (top <= 0)
            {
                top = 10;
            }

            var consulta = _contexto.ItensVenda
                .Include(i => i.ProdutoServico)
                .Include(i => i.Venda)
                .Where(i => i.Venda.Status == StatusVenda.Confirmada);

            if (inicio.HasValue)
            {
                consulta = consulta.Where(i => i.Venda.Data >= inicio.Value);
            }

            if (fim.HasValue)
            {
                var limite = fim.Value.Date.AddDays(1);
                consulta = consulta.Where(i => i.Venda.Data < limite);
            }

            var itens = await consulta.ToListAsync();

            return itens
                .GroupBy(i => new { i.ProdutoServicoId, Nome = i.ProdutoServico.Nome })
                .Select(g => new ProdutoMaisVendidoDto
                {
                    ProdutoServicoId = g.Key.ProdutoServicoId,
                    Produto = g.Key.Nome,
                    QuantidadeVendida = g.Sum(i => i.Quantidade),
                    TotalVendido = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.QuantidadeVendida)
                .Take(top)
                .ToList();
        }

        public async Task<TotalRoyaltiesDto> TotalRoyalties(int? ano, int? mes)
        {
            var consulta = _contexto.Royalties.AsQueryable();

            if (ano.HasValue)
            {
                consulta = consulta.Where(r => r.Ano == ano.Value);
            }

            if (mes.HasValue)
            {
                consulta = consulta.Where(r => r.Mes == mes.Value);
            }

            var royalties = await consulta.ToListAsync();

            return new TotalRoyaltiesDto
            {
                Ano = ano,
                Mes = mes,
                TotalGerado = royalties.Sum(r => r.ValorDevido),
                TotalPago = royalties.Where(r => r.StatusPagamento == StatusPagamento.Pago).Sum(r => r.ValorDevido),
                TotalPendente = royalties.Where(r => r.StatusPagamento == StatusPagamento.Pendente).Sum(r => r.ValorDevido)
            };
        }

        public async Task<List<EstoqueRespostaDto>> EstoqueCritico()
        {
            return await _estoqueService.ItensAbaixoDoMinimo(null);
        }

        public async Task<List<ChamadosPorStatusDto>> ChamadosPorStatus()
        {
            var chamados = await _contexto.Chamados.ToListAsync();

            return chamados
                .GroupBy(c => c.Status)
                .Select(g => new ChamadosPorStatusDto
                {
                    Status = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToList();
        }
    }
}
