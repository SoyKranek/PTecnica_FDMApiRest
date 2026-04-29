using CatalogoProductos.Application.Comandos;
using CatalogoProductos.Application.Contratos;
using CatalogoProductos.Application.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoProductos.API.Controllers;

[ApiController]
[Route("api/v1/productos/{idProducto:guid}/stock")]
public class StockController : ControllerBase
{
    private readonly IServicioGestionStock _servicioGestionStock;

    public StockController(IServicioGestionStock servicioGestionStock)
    {
        _servicioGestionStock = servicioGestionStock;
    }

    /// <summary>
    /// Ajusta el stock de un producto (suma o resta)
    /// </summary>
    /// <param name="idProducto">ID del producto</param>
    /// <param name="solicitud">Cantidad a ajustar y motivo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>El producto con el stock actualizado</returns>
    /// <response code="200">Stock ajustado exitosamente</response>
    /// <response code="400">Los datos son inválidos</response>
    /// <response code="404">El producto no existe</response>
    /// <response code="422">El ajuste dejaría el stock en negativo</response>
    [HttpPost("ajustar")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AjustarStock(Guid idProducto, [FromBody] AjustarStockRequest solicitud, CancellationToken cancellationToken = default)
    {
        if (idProducto == Guid.Empty)
        {
            throw new ArgumentException("El ID del producto no puede ser vacío.", nameof(idProducto));
        }

        var respuesta = await _servicioGestionStock.AjustarStockAsync(new AjustarStockCommand(idProducto, solicitud), cancellationToken);
        return Ok(respuesta);
    }
}
