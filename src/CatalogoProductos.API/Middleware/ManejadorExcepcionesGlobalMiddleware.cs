using CatalogoProductos.API.Models;
using CatalogoProductos.Domain.Excepciones;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CatalogoProductos.API.Middleware;

public class ManejadorExcepcionesGlobalMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorExcepcionesGlobalMiddleware> _logger;

    public ManejadorExcepcionesGlobalMiddleware(RequestDelegate siguiente, ILogger<ManejadorExcepcionesGlobalMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contextoHttp)
    {
        try
        {
            await _siguiente(contextoHttp);
        }
        catch (Exception excepcionCapturada)
        {
            _logger.LogError(excepcionCapturada, "Excepción no controlada: {Mensaje}", excepcionCapturada.Message);
            await ManejarExcepcionAsync(contextoHttp, excepcionCapturada);
        }
    }

    private static Task ManejarExcepcionAsync(HttpContext contextoHttp, Exception excepcionCapturada)
    {
        var respuesta = CrearRespuestaError(contextoHttp, excepcionCapturada);
        contextoHttp.Response.ContentType = "application/json";
        contextoHttp.Response.StatusCode = respuesta.CodigoEstado;

        var opciones = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return contextoHttp.Response.WriteAsync(JsonSerializer.Serialize(respuesta, opciones));
    }

    private static RespuestaError CrearRespuestaError(HttpContext contextoHttp, Exception excepcionCapturada)
    {
        return excepcionCapturada switch
        {
            ProductoNoEncontradoException => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.NotFound,
                CodigoError = CodigosError.PRODUCTO_NO_ENCONTRADO,
                Mensaje = excepcionCapturada.Message,
                Sugerencia = "Verifique que el ID del producto sea correcto y que el producto exista en el sistema.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            },

            StockInsuficienteException => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.UnprocessableEntity,
                CodigoError = CodigosError.STOCK_INSUFICIENTE,
                Mensaje = excepcionCapturada.Message,
                Sugerencia = "Verifique la cantidad en stock disponible antes de realizar el ajuste.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            },

            SkuDuplicadoException => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.Conflict,
                CodigoError = CodigosError.SKU_DUPLICADO,
                Mensaje = excepcionCapturada.Message,
                Sugerencia = "Utilice un código SKU diferente y único para este producto.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            },

            ReglaNegocioException => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.UnprocessableEntity,
                CodigoError = CodigosError.REGLA_NEGOCIO_VIOLADA,
                Mensaje = excepcionCapturada.Message,
                Sugerencia = "Revise los datos enviados y asegúrese de cumplir con las reglas de negocio.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            },

            ValidationException validacionEx => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.BadRequest,
                CodigoError = CodigosError.VALIDACION_FALLIDA,
                Mensaje = "Los datos enviados no cumplen con las validaciones requeridas.",
                Errores = validacionEx.Errors.Select(e => new ErrorValidacion
                {
                    Campo = e.PropertyName,
                    Mensaje = e.ErrorMessage
                }).ToList(),
                Sugerencia = "Corrija los campos indicados y vuelva a intentar.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            },

            ArgumentException => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.BadRequest,
                CodigoError = CodigosError.PARAMETROS_INVALIDOS,
                Mensaje = excepcionCapturada.Message,
                Sugerencia = "Verifique que todos los parámetros sean válidos y estén en el formato correcto.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            },

            _ => new RespuestaError
            {
                CodigoEstado = (int)HttpStatusCode.InternalServerError,
                CodigoError = CodigosError.ERROR_INTERNO,
                Mensaje = "Ocurrió un error inesperado en el servidor.",
                Detalles = excepcionCapturada.InnerException?.Message,
                Sugerencia = "Si el problema persiste, contacte al administrador del sistema.",
                MarcaTiempo = DateTime.UtcNow,
                RutaSolicitud = contextoHttp.Request.Path.Value ?? string.Empty
            }
        };
    }
}
