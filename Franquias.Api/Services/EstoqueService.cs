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
    public interface IEstoqueService
    {
        Task<EstoqueRespostaDto> Criar(EstoqueCriacaoDto dto);
        Task<EstoqueRespostaDto> Movimentar(MovimentacaoEstoqueDto dto);
        Task<EstoqueRespostaDto> ObterPorId(int id);
        Task<List<EstoqueRespostaDto>> ListarPorUnidade(int unidadeId);
        Task<List<EstoqueRespostaDto>> ItensAbaixoDoMinimo(int? unidadeId);
        Task<List<MovimentacaoRespostaDto>> ListarMovimentacoes(int estoqueId);
    }

    public class EstoqueService : IEstoqueService
    {
        private readonly AppDbContext _contexto;

        public EstoqueService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<EstoqueRespostaDto> Criar(EstoqueCriacaoDto dto)
        {
            var unidade = await _contexto.Unidades.FindAsync(dto.UnidadeFranqueadaId);
            if (unidade == null)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            var produto = await _contexto.Produtos.FindAsync(dto.ProdutoServicoId);
            if (produto == null)
            {
                throw new NaoEncontradoException("Produto ou servico nao encontrado.");
            }

            var jaExiste = await _contexto.Estoques.AnyAsync(e =>
                e.UnidadeFranqueadaId == dto.UnidadeFranqueadaId && e.ProdutoServicoId == dto.ProdutoServicoId);
            if (jaExiste)
            {
                throw new RegraNegocioException("Ja existe um registro de estoque para este produto nesta unidade.");
            }

            var estoque = new Estoque
            {
                UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
                ProdutoServicoId = dto.ProdutoServicoId,
                Quantidade = dto.QuantidadeInicial,
                QuantidadeMinima = dto.QuantidadeMinima
            };

            if (dto.QuantidadeInicial > 0)
            {
                estoque.Movimentacoes.Add(new MovimentacaoEstoque
                {
                    Tipo = TipoMovimentacao.Entrada,
                    Quantidade = dto.QuantidadeInicial,
                    Observacao = "Cadastro inicial do estoque"
                });
            }

            _contexto.Estoques.Add(estoque);
            await _contexto.SaveChangesAsync();

            return await ObterPorId(estoque.Id);
        }

        public async Task<EstoqueRespostaDto> Movimentar(MovimentacaoEstoqueDto dto)
        {
            var estoque = await _contexto.Estoques.FindAsync(dto.EstoqueId);
            if (estoque == null)
            {
                throw new NaoEncontradoException("Registro de estoque nao encontrado.");
            }

            if (dto.Tipo == TipoMovimentacao.Entrada)
            {
                estoque.Quantidade += dto.Quantidade;
            }
            else
            {
                if (estoque.Quantidade - dto.Quantidade < 0)
                {
                    throw new RegraNegocioException("A movimentacao deixaria o estoque negativo.");
                }

                estoque.Quantidade -= dto.Quantidade;
            }

            _contexto.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                EstoqueId = estoque.Id,
                Tipo = dto.Tipo,
                Quantidade = dto.Quantidade,
                Observacao = dto.Observacao
            });

            await _contexto.SaveChangesAsync();

            return await ObterPorId(estoque.Id);
        }

        public async Task<EstoqueRespostaDto> ObterPorId(int id)
        {
            var estoque = await _contexto.Estoques
                .Include(e => e.UnidadeFranqueada)
                .Include(e => e.ProdutoServico)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estoque == null)
            {
                throw new NaoEncontradoException("Registro de estoque nao encontrado.");
            }

            return Converter(estoque);
        }

        public async Task<List<EstoqueRespostaDto>> ListarPorUnidade(int unidadeId)
        {
            var unidadeExiste = await _contexto.Unidades.AnyAsync(u => u.Id == unidadeId);
            if (!unidadeExiste)
            {
                throw new NaoEncontradoException("Unidade nao encontrada.");
            }

            var estoques = await _contexto.Estoques
                .Include(e => e.UnidadeFranqueada)
                .Include(e => e.ProdutoServico)
                .Where(e => e.UnidadeFranqueadaId == unidadeId)
                .OrderBy(e => e.ProdutoServico.Nome)
                .ToListAsync();

            return estoques.Select(Converter).ToList();
        }

        public async Task<List<EstoqueRespostaDto>> ItensAbaixoDoMinimo(int? unidadeId)
        {
            var consulta = _contexto.Estoques
                .Include(e => e.UnidadeFranqueada)
                .Include(e => e.ProdutoServico)
                .Where(e => e.Quantidade < e.QuantidadeMinima);

            if (unidadeId.HasValue)
            {
                consulta = consulta.Where(e => e.UnidadeFranqueadaId == unidadeId.Value);
            }

            var estoques = await consulta
                .OrderBy(e => e.UnidadeFranqueada.Nome)
                .ThenBy(e => e.ProdutoServico.Nome)
                .ToListAsync();

            return estoques.Select(Converter).ToList();
        }

        public async Task<List<MovimentacaoRespostaDto>> ListarMovimentacoes(int estoqueId)
        {
            var estoqueExiste = await _contexto.Estoques.AnyAsync(e => e.Id == estoqueId);
            if (!estoqueExiste)
            {
                throw new NaoEncontradoException("Registro de estoque nao encontrado.");
            }

            var movimentacoes = await _contexto.MovimentacoesEstoque
                .Where(m => m.EstoqueId == estoqueId)
                .OrderByDescending(m => m.Data)
                .ToListAsync();

            return movimentacoes.Select(m => new MovimentacaoRespostaDto
            {
                Id = m.Id,
                Tipo = m.Tipo.ToString(),
                Quantidade = m.Quantidade,
                Observacao = m.Observacao,
                Data = m.Data
            }).ToList();
        }

        private static EstoqueRespostaDto Converter(Estoque estoque)
        {
            return new EstoqueRespostaDto
            {
                Id = estoque.Id,
                UnidadeFranqueadaId = estoque.UnidadeFranqueadaId,
                Unidade = estoque.UnidadeFranqueada != null ? estoque.UnidadeFranqueada.Nome : null,
                ProdutoServicoId = estoque.ProdutoServicoId,
                Produto = estoque.ProdutoServico != null ? estoque.ProdutoServico.Nome : null,
                Quantidade = estoque.Quantidade,
                QuantidadeMinima = estoque.QuantidadeMinima,
                AbaixoDoMinimo = estoque.Quantidade < estoque.QuantidadeMinima
            };
        }
    }
}
