using System;
using System.Text.Json;
using System.Threading.Tasks;
using Franquias.Api.Common;
using Microsoft.AspNetCore.Http;

namespace Franquias.Api.Middleware
{
    public class TratamentoErroMiddleware
    {
        private readonly RequestDelegate _proximo;

        public TratamentoErroMiddleware(RequestDelegate proximo)
        {
            _proximo = proximo;
        }

        public async Task Invoke(HttpContext contexto)
        {
            try
            {
                await _proximo(contexto);
            }
            catch (NaoEncontradoException ex)
            {
                await Responder(contexto, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (RegraNegocioException ex)
            {
                await Responder(contexto, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                await Responder(contexto, StatusCodes.Status500InternalServerError, "Erro interno no servidor: " + ex.Message);
            }
        }

        private static async Task Responder(HttpContext contexto, int status, string mensagem)
        {
            contexto.Response.ContentType = "application/json";
            contexto.Response.StatusCode = status;

            var corpo = JsonSerializer.Serialize(new
            {
                status = status,
                mensagem = mensagem
            });

            await contexto.Response.WriteAsync(corpo);
        }
    }
}
