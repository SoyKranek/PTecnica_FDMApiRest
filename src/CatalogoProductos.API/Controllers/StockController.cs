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

    [HttpPost("ajustar")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AjustarStock(Guid idProducto, [FromBody] AjustarStockRequest solicitud, CancellationToken cancellationToken = default)
    {
        var respuesta = await _servicioGestionStock.AjustarStockAsync(new AjustarStockCommand(idProducto, solicitud), cancellationToken);
        return Ok(respuesta);
    }
}
