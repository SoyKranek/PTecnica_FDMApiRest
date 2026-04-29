using CatalogoProductos.Domain.Entidades;
using CatalogoProductos.Domain.Excepciones;
using FluentAssertions;

namespace CatalogoProductos.Application.Tests;

/// <summary>
/// Tests para validar reglas de negocio de la entidad Producto
/// Intentan violar invariantes del dominio
/// </summary>
public class TestsDominioProducto
{
    [Fact]
    public void CrearProducto_ConPrecioNegativo_DebeLanzarExcepcion()
    {
        // Arrange & Act
        Action accion = () => new Producto(
            "Producto",
            "Descripción",
            -100.00m, // Precio negativo
            10,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Assert
        accion.Should().Throw<ReglaNegocioException>()
            .WithMessage("*precio*");
    }

    [Fact]
    public void CrearProducto_ConPrecioCero_DebeLanzarExcepcion()
    {
        // Arrange & Act
        Action accion = () => new Producto(
            "Producto",
            "Descripción",
            0.00m,
            10,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Assert
        accion.Should().Throw<ReglaNegocioException>()
            .WithMessage("*precio*");
    }

    [Fact]
    public void CrearProducto_ConStockNegativo_DebeLanzarExcepcion()
    {
        // Arrange & Act
        Action accion = () => new Producto(
            "Producto",
            "Descripción",
            100.00m,
            -5, // Stock negativo
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Assert
        accion.Should().Throw<ReglaNegocioException>()
            .WithMessage("*stock*");
    }

    [Fact]
    public void CrearProducto_ConDatosValidos_DebeCrearCorrectamente()
    {
        // Arrange & Act
        var producto = new Producto(
            "Laptop HP",
            "Laptop profesional",
            2500.00m,
            10,
            "Tecnología",
            "https://example.com/laptop.jpg",
            "LAP-HP-001"
        );

        // Assert
        producto.Nombre.Should().Be("Laptop HP");
        producto.Precio.Should().Be(2500.00m);
        producto.CantidadStock.Should().Be(10);
        producto.EstaActivo.Should().BeTrue();
        producto.IdProducto.Should().NotBe(Guid.Empty);
        producto.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AjustarStock_ConAjustePositivo_DebeIncrementar()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            50,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act
        producto.AjustarStock(25);

        // Assert
        producto.CantidadStock.Should().Be(75);
        producto.FechaActualizacion.Should().NotBeNull();
    }

    [Fact]
    public void AjustarStock_ConAjusteNegativo_DebeDecrementar()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            50,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act
        producto.AjustarStock(-25);

        // Assert
        producto.CantidadStock.Should().Be(25);
    }

    [Fact]
    public void AjustarStock_QueDejaNegativo_DebeLanzarExcepcion()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            50,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act
        Action accion = () => producto.AjustarStock(-100); // Dejaría -50

        // Assert
        accion.Should().Throw<StockInsuficienteException>()
            .WithMessage("*negativo*");
    }

    [Fact]
    public void AjustarStock_HastaCero_DebePermitir()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            50,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act
        producto.AjustarStock(-50);

        // Assert
        producto.CantidadStock.Should().Be(0);
    }

    [Fact]
    public void ActualizarDatos_ConPrecioNegativo_DebeLanzarExcepcion()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            50,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act
        Action accion = () => producto.ActualizarDatos(
            "Nuevo nombre",
            "Nueva descripción",
            -50.00m, // Precio negativo
            "Nueva categoría",
            "https://example.com/new.jpg"
        );

        // Assert
        accion.Should().Throw<ReglaNegocioException>()
            .WithMessage("*precio*");
    }

    [Fact]
    public void ActualizarDatos_ConDatosValidos_DebeActualizar()
    {
        // Arrange
        var producto = new Producto(
            "Producto Original",
            "Descripción Original",
            100.00m,
            50,
            "Categoría Original",
            "https://example.com/old.jpg",
            "SKU-001"
        );

        // Act
        producto.ActualizarDatos(
            "Producto Actualizado",
            "Descripción Actualizada",
            150.00m,
            "Categoría Nueva",
            "https://example.com/new.jpg"
        );

        // Assert
        producto.Nombre.Should().Be("Producto Actualizado");
        producto.Descripcion.Should().Be("Descripción Actualizada");
        producto.Precio.Should().Be(150.00m);
        producto.Categoria.Should().Be("Categoría Nueva");
        producto.UrlImagen.Should().Be("https://example.com/new.jpg");
        producto.FechaActualizacion.Should().NotBeNull();
        producto.FechaActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarcarComoInactivo_DebeDesactivarProducto()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            50,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act
        producto.MarcarComoInactivo();

        // Assert
        producto.EstaActivo.Should().BeFalse();
        producto.FechaActualizacion.Should().NotBeNull();
    }

    [Fact]
    public void AjustarStock_ConInt32MaxValue_DebeManejarOverflow()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            int.MaxValue - 10,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act - Intentar overflow
        Action accion = () => producto.AjustarStock(100);

        // Assert - Debería lanzar excepción por overflow
        accion.Should().Throw<Exception>();
    }

    [Fact]
    public void MultipleAjustesSecuenciales_DebenSerConsistentes()
    {
        // Arrange
        var producto = new Producto(
            "Producto",
            "Descripción",
            100.00m,
            100,
            "Categoría",
            "https://example.com/img.jpg",
            "SKU-001"
        );

        // Act - Simular múltiples operaciones
        producto.AjustarStock(50);   // 150
        producto.AjustarStock(-30);  // 120
        producto.AjustarStock(10);   // 130
        producto.AjustarStock(-40);  // 90

        // Assert
        producto.CantidadStock.Should().Be(90);
    }
}
