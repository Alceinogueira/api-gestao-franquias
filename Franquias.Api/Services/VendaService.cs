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
    public interface IVendaService
    {
        Task<VendaRespostaDto> Criar(VendaCriacaoDto dto, int usuarioId);
        Task<VendaRespostaDto> Confirmar(int id);
        Task<VendaRespostaDto> Cancelar(int id);
        Task<VendaRespostaDto> ObterPorId(int id);
        Task<List<VendaRespostaDto>> Listar(int? unidadeId, DateTime? inicio, DateTime? fim);
    }

    public class VendaService : IVendaService
    {
        private readonly AppDbContext _contexto;

        public VendaService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<VendaRespostaDto> Criar(VendaCriacaoDto dto, int usuarioId)
        {
            if (dto.Itens == null || dto.Itens.Count == 0)
            {
                throw new RegraNegocioException("A venda deve possuir pelo menos um item.");
            }

            var unidade = await _contexto.Unidades.FindAsync(dto.UnidadeFranqueadaId);
            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            if (!unidade.Ativa)
            {
                throw new RegraNegocioException("Unidade inativa nao pode registrar novas vendas.");
            }

            var venda = new Venda
            {
                UnidadeFranqueadaId = unidade.Id,
                UsuarioId = usuarioId,
                Data = DateTime.UtcNow,
                Status = StatusVenda.Aberta
            };

            decimal total = 0;

            // agrupa itens repetidos pelo mesmo produto
            var itensAgrupados = dto.Itens
                .GroupBy(i => i.ProdutoServicoId)
                .Select(g => new { ProdutoServicoId = g.Key, Quantidade = g.Sum(x => x.Quantidade) });

            foreach (var item in itensAgrupados)
            {
                var produto = await _contexto.Produtos.FindAsync(item.ProdutoServicoId);
                if (produto == null)
                {
                    throw new NaoEncontradoException("Produto ou servico " + item.ProdutoServicoId + " nao encontrado.");
                }

                if (!produto.Ativo)
                {
                    throw new RegraNegocioException("O produto '" + produto.Nome + "' esta inativo.");
                }

                if (item.Quantidade <= 0)
                {
                    throw new RegraNegocioException("A quantidade dos itens deve ser maior que zero.");
                }

                var subtotal = produto.PrecoBase * item.Quantidade;
                total += subtotal;

                venda.Itens.Add(new ItemVenda
                {
                    ProdutoServicoId = produto.Id,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = produto.PrecoBase,
                    Subtotal = subtotal
                });
            }

            venda.ValorTotal = total;

            _contexto.Vendas.Add(venda);
            await _contexto.SaveChangesAsync();

            return await ObterPorId(venda.Id);
        }

        public async Task<VendaRespostaDto> Confirmar(int id)
        {
            var venda = await _contexto.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                throw new NaoEncontradoException("Venda nao encontrada.");
            }

            if (venda.Status != StatusVenda.Aberta)
            {
                throw new RegraNegocioException("Somente vendas em aberto podem ser confirmadas.");
            }

            // valida o estoque de todos os itens que sao produtos (servico nao movimenta estoque)
            foreach (var item in venda.Itens)
            {
                var produto = await _contexto.Produtos.FindAsync(item.ProdutoServicoId);
                if (produto != null && produto.EhServico)
                {
                    continue;
                }

                var estoque = await _contexto.Estoques.FirstOrDefaultAsync(e =>
                    e.UnidadeFranqueadaId == venda.UnidadeFranqueadaId && e.ProdutoServicoId == item.ProdutoServicoId);

                if (estoque == null)
                {
                    throw new RegraNegocioException("Nao ha estoque cadastrado para o produto " + item.ProdutoServicoId + " nesta unidade.");
                }

                if (estoque.Quantidade - item.Quantidade < 0)
                {
                    throw new RegraNegocioException("Estoque insuficiente para o produto " + item.ProdutoServicoId + ".");
                }
            }

            // aplica a baixa de estoque
            foreach (var item in venda.Itens)
            {
                var estoque = await _contexto.Estoques.FirstOrDefaultAsync(e =>
                    e.UnidadeFranqueadaId == venda.UnidadeFranqueadaId && e.ProdutoServicoId == item.ProdutoServicoId);

                if (estoque == null)
                {
                    continue;
                }

                estoque.Quantidade -= item.Quantidade;

                _contexto.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    EstoqueId = estoque.Id,
                    Tipo = TipoMovimentacao.Saida,
                    Quantidade = item.Quantidade,
                    Observacao = "Baixa pela venda " + venda.Id
                });
            }

            venda.Status = StatusVenda.Confirmada;
            await _contexto.SaveChangesAsync();

            return await ObterPorId(venda.Id);
        }

        public async Task<VendaRespostaDto> Cancelar(int id)
        {
            var venda = await _contexto.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                throw new NaoEncontradoException("Venda nao encontrada.");
            }

            if (venda.Status == StatusVenda.Cancelada)
            {
                throw new RegraNegocioException("A venda ja esta cancelada.");
            }

            // se ja estava confirmada, devolve os itens ao estoque
            if (venda.Status == StatusVenda.Confirmada)
            {
                foreach (var item in venda.Itens)
                {
                    var estoque = await _contexto.Estoques.FirstOrDefaultAsync(e =>
                        e.UnidadeFranqueadaId == venda.UnidadeFranqueadaId && e.ProdutoServicoId == item.ProdutoServicoId);

                    if (estoque == null)
                    {
                        continue;
                    }

                    estoque.Quantidade += item.Quantidade;

                    _contexto.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                    {
                        EstoqueId = estoque.Id,
                        Tipo = TipoMovimentacao.Entrada,
                        Quantidade = item.Quantidade,
                        Observacao = "Estorno do cancelamento da venda " + venda.Id
                    });
                }
            }

            venda.Status = StatusVenda.Cancelada;
            await _contexto.SaveChangesAsync();

            return await ObterPorId(venda.Id);
        }

        public async Task<VendaRespostaDto> ObterPorId(int id)
        {
            var venda = await _contexto.Vendas
                .Include(v => v.UnidadeFranqueada)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.ProdutoServico)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                throw new NaoEncontradoException("Venda nao encontrada.");
            }

            return Converter(venda);
        }

        public async Task<List<VendaRespostaDto>> Listar(int? unidadeId, DateTime? inicio, DateTime? fim)
        {
            var consulta = _contexto.Vendas
                .Include(v => v.UnidadeFranqueada)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.ProdutoServico)
                .AsQueryable();

            if (unidadeId.HasValue)
            {
                consulta = consulta.Where(v => v.UnidadeFranqueadaId == unidadeId.Value);
            }

            if (inicio.HasValue)
            {
                consulta = consulta.Where(v => v.Data >= inicio.Value);
            }

            if (fim.HasValue)
            {
                var limite = fim.Value.Date.AddDays(1);
                consulta = consulta.Where(v => v.Data < limite);
            }

            var vendas = await consulta.OrderByDescending(v => v.Data).ToListAsync();
            return vendas.Select(Converter).ToList();
        }

        private static VendaRespostaDto Converter(Venda venda)
        {
            return new VendaRespostaDto
            {
                Id = venda.Id,
                UnidadeFranqueadaId = venda.UnidadeFranqueadaId,
                Unidade = venda.UnidadeFranqueada != null ? venda.UnidadeFranqueada.Nome : null,
                Data = venda.Data,
                ValorTotal = venda.ValorTotal,
                Status = venda.Status.ToString(),
                UsuarioId = venda.UsuarioId,
                Itens = venda.Itens.Select(i => new ItemVendaRespostaDto
                {
                    ProdutoServicoId = i.ProdutoServicoId,
                    Produto = i.ProdutoServico != null ? i.ProdutoServico.Nome : null,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }
    }
}
