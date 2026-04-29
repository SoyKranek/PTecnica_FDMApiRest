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

    /// <summary>
    /// Obtiene un listado paginado de productos activos
    /// </summary>
    /// <param name="pagina">Número de página (debe ser mayor a 0)</param>
    /// <param name="tamano">Cantidad de elementos por página (entre 1 y 100)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de productos</returns>
    /// <response code="200">Retorna la lista de productos paginada</response>
    /// <response code="400">Los parámetros de paginación son inválidos</response>
    [HttpGet]
    [ProducesResponseType(typeof(RespuestaPaginada<ProductoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerProductos([FromQuery] int pagina = 1, [FromQuery] int tamano = 10, CancellationToken cancellationToken = default)
    {
        if (pagina <= 0)
        {
            throw new ArgumentException("El número de página debe ser mayor a 0.", nameof(pagina));
        }

        if (tamano <= 0 || tamano > 100)
        {
            throw new ArgumentException("El tamaño de página debe estar entre 1 y 100.", nameof(tamano));
        }

        var respuesta = await _servicioGestionProductos.ObtenerProductosPaginadosAsync(
            new ObtenerProductosPaginadosQuery(pagina, tamano),
            cancellationToken);
        return Ok(respuesta);
    }

    /// <summary>
    /// Obtiene un producto específico por su ID
    /// </summary>
    /// <param name="idProducto">ID único del producto (GUID)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Detalle completo del producto</returns>
    /// <response code="200">Retorna el producto encontrado</response>
    /// <response code="404">El producto no existe</response>
    [HttpGet("{idProducto:guid}")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerProductoPorId(Guid idProducto, CancellationToken cancellationToken = default)
    {
        if (idProducto == Guid.Empty)
        {
            throw new ArgumentException("El ID del producto no puede ser vacío.", nameof(idProducto));
        }

        var respuesta = await _servicioGestionProductos.ObtenerProductoPorIdAsync(new ObtenerProductoPorIdQuery(idProducto), cancellationToken);
        return Ok(respuesta);
    }

    /// <summary>
    /// Crea un nuevo producto en el catálogo
    /// </summary>
    /// <param name="solicitud">Datos del producto a crear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>El producto creado con su ID asignado</returns>
    /// <response code="201">Producto creado exitosamente</response>
    /// <response code="400">Los datos del producto son inválidos</response>
    /// <response code="409">El código SKU ya existe</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearProducto([FromBody] CrearProductoRequest solicitud, CancellationToken cancellationToken = default)
    {
        var respuesta = await _servicioGestionProductos.CrearProductoAsync(new CrearProductoCommand(solicitud), cancellationToken);
        return CreatedAtAction(nameof(ObtenerProductoPorId), new { idProducto = respuesta.IdProducto }, respuesta);
    }

    /// <summary>
    /// Actualiza los datos de un producto existente
    /// </summary>
    /// <param name="idProducto">ID del producto a actualizar</param>
    /// <param name="solicitud">Nuevos datos del producto</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>El producto actualizado</returns>
    /// <response code="200">Producto actualizado exitosamente</response>
    /// <response code="400">Los datos son inválidos</response>
    /// <response code="404">El producto no existe</response>
    [HttpPut("{idProducto:guid}")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarProducto(Guid idProducto, [FromBody] ActualizarProductoRequest solicitud, CancellationToken cancellationToken = default)
    {
        if (idProducto == Guid.Empty)
        {
            throw new ArgumentException("El ID del producto no puede ser vacío.", nameof(idProducto));
        }

        var respuesta = await _servicioGestionProductos.ActualizarProductoAsync(
            new ActualizarProductoCommand(idProducto, solicitud),
            cancellationToken);
        return Ok(respuesta);
    }

    /// <summary>
    /// Elimina (marca como inactivo) un producto del catálogo
    /// </summary>
    /// <param name="idProducto">ID del producto a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Sin contenido</returns>
    /// <response code="204">Producto eliminado exitosamente</response>
    /// <response code="404">El producto no existe</response>
    [HttpDelete("{idProducto:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarProducto(Guid idProducto, CancellationToken cancellationToken = default)
    {
        if (idProducto == Guid.Empty)
        {
            throw new ArgumentException("El ID del producto no puede ser vacío.", nameof(idProducto));
        }

        await _servicioGestionProductos.EliminarProductoAsync(new EliminarProductoCommand(idProducto), cancellationToken);
        return NoContent();
    }
}
