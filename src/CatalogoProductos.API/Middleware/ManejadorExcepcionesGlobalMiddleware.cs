using CatalogoProductos.Domain.Excepciones;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CatalogoProductos.API.Middleware;

public class ManejadorExcepcionesGlobalMiddleware
{
    private readonly RequestDelegate _siguiente;

    public ManejadorExcepcionesGlobalMiddleware(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public async Task InvokeAsync(HttpContext contextoHttp)
    {
        try
        {
            await _siguiente(contextoHttp);
        }
        catch (Exception excepcionCapturada)
        {
            await ManejarExcepcionAsync(contextoHttp, excepcionCapturada);
        }
    }

    private static Task ManejarExcepcionAsync(HttpContext contextoHttp, Exception excepcionCapturada)
    {
        var codigoEstado = excepcionCapturada switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ProductoNoEncontradoException => HttpStatusCode.NotFound,
            StockInsuficienteException => HttpStatusCode.BadRequest,
            ReglaNegocioException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        var respuesta = new
        {
            codigoEstado = (int)codigoEstado,
            mensaje = excepcionCapturada.Message,
            detalles = excepcionCapturada.InnerException?.Message,
            marcaTiempo = DateTime.UtcNow,
            rutaSolicitud = contextoHttp.Request.Path.Value
        };

        contextoHttp.Response.ContentType = "application/json";
        contextoHttp.Response.StatusCode = (int)codigoEstado;
        return contextoHttp.Response.WriteAsync(JsonSerializer.Serialize(respuesta));
    }
}
