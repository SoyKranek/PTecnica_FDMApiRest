using CatalogoProductos.Domain.Excepciones;

namespace CatalogoProductos.Domain.Entidades
{
public class Producto
{
    public Guid IdProducto { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Precio { get; private set; }
    public int CantidadStock { get; private set; }
    public string Categoria { get; private set; }
    public string UrlImagen { get; private set; }
    public string CodigoSKU { get; private set; }
    public bool EstaActivo { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaActualizacion { get; private set; }

    private Producto()
    {
        Nombre = string.Empty;
        Descripcion = string.Empty;
        Categoria = string.Empty;
        UrlImagen = string.Empty;
        CodigoSKU = string.Empty;
    }

    public Producto(
        string nombre,
        string descripcion,
        decimal precio,
        int cantidadStock,
        string categoria,
        string urlImagen,
        string codigoSku)
    {
        if (precio <= 0)
        {
            throw new ReglaNegocioException("El precio debe ser mayor a cero.");
        }

        if (cantidadStock < 0)
        {
            throw new ReglaNegocioException("La cantidad de stock no puede ser negativa.");
        }

        IdProducto = Guid.NewGuid();
        Nombre = nombre;
        Descripcion = descripcion;
        Precio = precio;
        CantidadStock = cantidadStock;
        Categoria = categoria;
        UrlImagen = urlImagen;
        CodigoSKU = codigoSku;
        EstaActivo = true;
        FechaCreacion = DateTime.UtcNow;
    }

    public void ActualizarDatos(
        string nombre,
        string descripcion,
        decimal precio,
        string categoria,
        string urlImagen)
    {
        if (precio <= 0)
        {
            throw new ReglaNegocioException("El precio debe ser mayor a cero.");
        }

        Nombre = nombre;
        Descripcion = descripcion;
        Precio = precio;
        Categoria = categoria;
        UrlImagen = urlImagen;
        FechaActualizacion = DateTime.UtcNow;
    }

    public void AjustarStock(int cantidadAjuste)
    {
        var nuevoStock = CantidadStock + cantidadAjuste;
        if (nuevoStock < 0)
        {
            throw new StockInsuficienteException("La operación dejaría el stock en negativo.");
        }

        CantidadStock = nuevoStock;
        FechaActualizacion = DateTime.UtcNow;
    }

    public void MarcarComoInactivo()
    {
        EstaActivo = false;
        FechaActualizacion = DateTime.UtcNow;
    }
}
}

namespace CatalogoProductos.Domain.Compartido
{
public class ResultadoPaginado<T>
{
    public required IReadOnlyCollection<T> Elementos { get; init; }
    public required int NumeroPagina { get; init; }
    public required int TamanoPagina { get; init; }
    public required int TotalRegistros { get; init; }
}
}

namespace CatalogoProductos.Domain.Interfaces
{
using CatalogoProductos.Domain.Compartido;
using CatalogoProductos.Domain.Entidades;

public interface IRepositorioProducto
{
    Task<Producto?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<Producto>> ObtenerPaginadoAsync(
        int numeroPagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);
    Task<bool> ExistePorCodigoSkuAsync(string codigoSku, CancellationToken cancellationToken = default);
    Task CrearAsync(Producto producto, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default);
    Task EliminarAsync(Producto producto, CancellationToken cancellationToken = default);
}

public interface IUnidadDeTrabajo
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
}

namespace CatalogoProductos.Domain.Excepciones
{
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}

public sealed class ProductoNoEncontradoException : ReglaNegocioException
{
    public ProductoNoEncontradoException(Guid idProducto)
        : base($"No se encontró el producto con id {idProducto}.")
    {
    }
}

public sealed class StockInsuficienteException : ReglaNegocioException
{
    public StockInsuficienteException(string mensaje) : base(mensaje)
    {
    }
}

public sealed class SkuDuplicadoException : ReglaNegocioException
{
    public SkuDuplicadoException(string codigoSku)
        : base($"Ya existe un producto con el código SKU '{codigoSku}'.")
    {
    }
}
}
