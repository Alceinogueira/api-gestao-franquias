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
    public interface IProdutoService
    {
        Task<ProdutoRespostaDto> Criar(ProdutoCriacaoDto dto);
        Task<ProdutoRespostaDto> Atualizar(int id, ProdutoAtualizacaoDto dto);
        Task<ProdutoRespostaDto> ObterPorId(int id);
        Task<List<ProdutoRespostaDto>> Listar(string nome, int? categoriaId, bool? ativo, bool? ehServico);
        Task<ProdutoRespostaDto> AlterarSituacao(int id, bool ativo);
    }

    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _contexto;

        public ProdutoService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<ProdutoRespostaDto> Criar(ProdutoCriacaoDto dto)
        {
            await ValidarRelacionamentos(dto.CategoriaId, dto.FornecedorId);

            var produto = new ProdutoServico
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                PrecoBase = dto.PrecoBase,
                EhServico = dto.EhServico,
                Ativo = true,
                CategoriaId = dto.CategoriaId,
                FornecedorId = dto.FornecedorId
            };

            _contexto.Produtos.Add(produto);
            await _contexto.SaveChangesAsync();

            return await ObterPorId(produto.Id);
        }

        public async Task<ProdutoRespostaDto> Atualizar(int id, ProdutoAtualizacaoDto dto)
        {
            var produto = await _contexto.Produtos.FindAsync(id);
            if (produto == null)
            {
                throw new NaoEncontradoException("Produto ou servico nao encontrado.");
            }

            await ValidarRelacionamentos(dto.CategoriaId, dto.FornecedorId);

            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.PrecoBase = dto.PrecoBase;
            produto.EhServico = dto.EhServico;
            produto.CategoriaId = dto.CategoriaId;
            produto.FornecedorId = dto.FornecedorId;

            await _contexto.SaveChangesAsync();

            return await ObterPorId(produto.Id);
        }

        public async Task<ProdutoRespostaDto> ObterPorId(int id)
        {
            var produto = await _contexto.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                throw new NaoEncontradoException("Produto ou servico nao encontrado.");
            }

            return Converter(produto);
        }

        public async Task<List<ProdutoRespostaDto>> Listar(string nome, int? categoriaId, bool? ativo, bool? ehServico)
        {
            var consulta = _contexto.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var termo = nome.Trim().ToLower();
                consulta = consulta.Where(p => p.Nome.ToLower().Contains(termo));
            }

            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
            }

            if (ativo.HasValue)
            {
                consulta = consulta.Where(p => p.Ativo == ativo.Value);
            }

            if (ehServico.HasValue)
            {
                consulta = consulta.Where(p => p.EhServico == ehServico.Value);
            }

            var produtos = await consulta.OrderBy(p => p.Nome).ToListAsync();
            return produtos.Select(Converter).ToList();
        }

        public async Task<ProdutoRespostaDto> AlterarSituacao(int id, bool ativo)
        {
            var produto = await _contexto.Produtos.FindAsync(id);
            if (produto == null)
            {
                throw new NaoEncontradoException("Produto ou servico nao encontrado.");
            }

            produto.Ativo = ativo;
            await _contexto.SaveChangesAsync();

            return await ObterPorId(produto.Id);
        }

        private async Task ValidarRelacionamentos(int categoriaId, int? fornecedorId)
        {
            var categoriaExiste = await _contexto.Categorias.AnyAsync(c => c.Id == categoriaId);
            if (!categoriaExiste)
            {
                throw new NaoEncontradoException("Categoria nao encontrada.");
            }

            if (fornecedorId.HasValue)
            {
                var fornecedorExiste = await _contexto.Fornecedores.AnyAsync(f => f.Id == fornecedorId.Value);
                if (!fornecedorExiste)
                {
                    throw new NaoEncontradoException("Fornecedor nao encontrado.");
                }
            }
        }

        private static ProdutoRespostaDto Converter(ProdutoServico produto)
        {
            return new ProdutoRespostaDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                PrecoBase = produto.PrecoBase,
                EhServico = produto.EhServico,
                Ativo = produto.Ativo,
                CategoriaId = produto.CategoriaId,
                Categoria = produto.Categoria != null ? produto.Categoria.Nome : null,
                FornecedorId = produto.FornecedorId,
                Fornecedor = produto.Fornecedor != null ? produto.Fornecedor.Nome : null
            };
        }
    }
}
