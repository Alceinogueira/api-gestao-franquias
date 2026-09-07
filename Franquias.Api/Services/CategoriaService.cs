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
    public interface ICategoriaService
    {
        Task<CategoriaRespostaDto> Criar(CategoriaCriacaoDto dto);
        Task<List<CategoriaRespostaDto>> Listar();
        Task<CategoriaRespostaDto> ObterPorId(int id);
        Task<CategoriaRespostaDto> AlterarSituacao(int id, bool ativa);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _contexto;

        public CategoriaService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<CategoriaRespostaDto> Criar(CategoriaCriacaoDto dto)
        {
            var nomeExiste = await _contexto.Categorias.AnyAsync(c => c.Nome.ToLower() == dto.Nome.ToLower());
            if (nomeExiste)
            {
                throw new RegraNegocioException("Ja existe uma categoria com este nome.");
            }

            var categoria = new Categoria
            {
                Nome = dto.Nome,
                Ativa = true
            };

            _contexto.Categorias.Add(categoria);
            await _contexto.SaveChangesAsync();

            return Converter(categoria);
        }

        public async Task<List<CategoriaRespostaDto>> Listar()
        {
            var categorias = await _contexto.Categorias.OrderBy(c => c.Nome).ToListAsync();
            return categorias.Select(Converter).ToList();
        }

        public async Task<CategoriaRespostaDto> ObterPorId(int id)
        {
            var categoria = await _contexto.Categorias.FindAsync(id);
            if (categoria == null)
            {
                throw new NaoEncontradoException("Categoria nao encontrada.");
            }

            return Converter(categoria);
        }

        public async Task<CategoriaRespostaDto> AlterarSituacao(int id, bool ativa)
        {
            var categoria = await _contexto.Categorias.FindAsync(id);
            if (categoria == null)
            {
                throw new NaoEncontradoException("Categoria nao encontrada.");
            }

            categoria.Ativa = ativa;
            await _contexto.SaveChangesAsync();

            return Converter(categoria);
        }

        private static CategoriaRespostaDto Converter(Categoria categoria)
        {
            return new CategoriaRespostaDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Ativa = categoria.Ativa
            };
        }
    }
}
