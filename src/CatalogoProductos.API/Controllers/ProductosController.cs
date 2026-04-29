using CatalogoProductos.Application.Comandos;
using CatalogoProductos.Application.Contratos;
using CatalogoProductos.Application.Consultas;
using CatalogoProductos.Application.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoProductos.API.Controllers;

[ApiController]
[Route("api/v1/productos")]
public class ProductosController : ControllerBase
{
    private readonly IServicioGestionProductos _servicioGestionProductos;

    public ProductosController(IServicioGestionProductos servicioGestionProductos)
    {
        _servicioGestionProductos = servicioGestionProductos;
    }

    [HttpGet]
    [ProducesResponseType(typeof(RespuestaPaginada<ProductoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerProductos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10, CancellationToken cancellationToken = default)
    {
        var respuesta = await _servicioGestionProductos.ObtenerProductosPaginadosAsync(
            new ObtenerProductosPaginadosQuery(pagina, tamano),
            cancellationToken);
        return Ok(respuesta);
    }

    [HttpGet("{idProducto:guid}")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerProductoPorId(Guid idProducto, CancellationToken cancellationToken = default)
    {
        var respuesta = await _servicioGestionProductos.ObtenerProductoPorIdAsync(new ObtenerProductoPorIdQuery(idProducto), cancellationToken);
        return Ok(respuesta);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearProducto([FromBody] CrearProductoRequest solicitud, CancellationToken cancellationToken = default)
    {
        var respuesta = await _servicioGestionProductos.CrearProductoAsync(new CrearProductoCommand(solicitud), cancellationToken);
        return CreatedAtAction(nameof(ObtenerProductoPorId), new { idProducto = respuesta.IdProducto }, respuesta);
    }

    [HttpPut("{idProducto:guid}")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarProducto(Guid idProducto, [FromBody] ActualizarProductoRequest solicitud, CancellationToken cancellationToken = default)
    {
        var respuesta = await _servicioGestionProductos.ActualizarProductoAsync(
            new ActualizarProductoCommand(idProducto, solicitud),
            cancellationToken);
        return Ok(respuesta);
    }

    [HttpDelete("{idProducto:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarProducto(Guid idProducto, CancellationToken cancellationToken = default)
    {
        await _servicioGestionProductos.EliminarProductoAsync(new EliminarProductoCommand(idProducto), cancellationToken);
        return NoContent();
    }
}
