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
    public interface IUsuarioService
    {
        Task<UsuarioRespostaDto> Registrar(RegistroUsuarioDto dto);
        Task<LoginRespostaDto> Login(LoginDto dto);
        Task<List<UsuarioRespostaDto>> Listar();
        Task<UsuarioRespostaDto> AlterarSituacao(int id, bool ativo);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _contexto;
        private readonly ITokenService _tokenService;

        public UsuarioService(AppDbContext contexto, ITokenService tokenService)
        {
            _contexto = contexto;
            _tokenService = tokenService;
        }

        public async Task<UsuarioRespostaDto> Registrar(RegistroUsuarioDto dto)
        {
            var emailExiste = await _contexto.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (emailExiste)
            {
                throw new RegraNegocioException("Ja existe um usuario com este e-mail.");
            }

            if (dto.Perfil != Perfil.AdministradorFranqueadora && dto.UnidadeFranqueadaId == null)
            {
                throw new RegraNegocioException("Usuarios de unidade precisam estar vinculados a uma unidade franqueada.");
            }

            if (dto.UnidadeFranqueadaId != null)
            {
                var unidadeExiste = await _contexto.Unidades.AnyAsync(u => u.Id == dto.UnidadeFranqueadaId);
                if (!unidadeExiste)
                {
                    throw new NaoEncontradoException("Unidade franqueada nao encontrada.");
                }
            }

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Perfil = dto.Perfil,
                Ativo = true,
                UnidadeFranqueadaId = dto.Perfil == Perfil.AdministradorFranqueadora ? null : dto.UnidadeFranqueadaId
            };

            _contexto.Usuarios.Add(usuario);
            await _contexto.SaveChangesAsync();

            return Converter(usuario);
        }

        public async Task<LoginRespostaDto> Login(LoginDto dto)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            {
                throw new RegraNegocioException("E-mail ou senha invalidos.");
            }

            if (!usuario.Ativo)
            {
                throw new RegraNegocioException("Usuario inativo. Contate o administrador.");
            }

            var token = _tokenService.GerarToken(usuario);

            return new LoginRespostaDto
            {
                Token = token,
                Nome = usuario.Nome,
                Perfil = usuario.Perfil.ToString(),
                ExpiraEm = DateTime.UtcNow.AddMinutes(_tokenService.MinutosExpiracao)
            };
        }

        public async Task<List<UsuarioRespostaDto>> Listar()
        {
            var usuarios = await _contexto.Usuarios
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return usuarios.Select(Converter).ToList();
        }

        public async Task<UsuarioRespostaDto> AlterarSituacao(int id, bool ativo)
        {
            var usuario = await _contexto.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                throw new NaoEncontradoException("Usuario nao encontrado.");
            }

            usuario.Ativo = ativo;
            await _contexto.SaveChangesAsync();

            return Converter(usuario);
        }

        private static UsuarioRespostaDto Converter(Usuario usuario)
        {
            return new UsuarioRespostaDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Ativo = usuario.Ativo,
                UnidadeFranqueadaId = usuario.UnidadeFranqueadaId
            };
        }
    }
}
