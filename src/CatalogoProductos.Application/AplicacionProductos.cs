using AutoMapper;
using CatalogoProductos.Domain.Entidades;
using CatalogoProductos.Domain.Excepciones;
using CatalogoProductos.Domain.Interfaces;
using FluentValidation;

namespace CatalogoProductos.Application.Contratos
{
    public class CrearProductoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CantidadStock { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
        public string CodigoSKU { get; set; } = string.Empty;
    }

    public class ActualizarProductoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
    }

    public class AjustarStockRequest
    {
        public int CantidadAjuste { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class ProductoResponse
    {
        public Guid IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CantidadStock { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
        public string CodigoSKU { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }

    public class RespuestaPaginada<T>
    {
        public required IReadOnlyCollection<T> Elementos { get; init; }
        public required int NumeroPagina { get; init; }
        public required int TamanoPagina { get; init; }
        public required int TotalRegistros { get; init; }
    }
}

namespace CatalogoProductos.Application.Comandos
{
    using CatalogoProductos.Application.Contratos;

    public record CrearProductoCommand(CrearProductoRequest Solicitud);
    public record ActualizarProductoCommand(Guid IdProducto, ActualizarProductoRequest Solicitud);
    public record EliminarProductoCommand(Guid IdProducto);
    public record AjustarStockCommand(Guid IdProducto, AjustarStockRequest Solicitud);
}

namespace CatalogoProductos.Application.Consultas
{
    public record ObtenerProductoPorIdQuery(Guid IdProducto);
    public record ObtenerProductosPaginadosQuery(int NumeroPagina, int TamanoPagina);
}

namespace CatalogoProductos.Application.Validadores
{
    using CatalogoProductos.Application.Contratos;

    public class CrearProductoRequestValidator : AbstractValidator<CrearProductoRequest>
    {
        public CrearProductoRequestValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.")
                .Must(NoContenerCaracteresEspeciales).WithMessage("El nombre no puede contener caracteres especiales peligrosos.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres.");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a cero.")
                .LessThan(10000000).WithMessage("El precio no puede exceder 10,000,000.");

            RuleFor(x => x.CantidadStock)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de stock no puede ser negativa.")
                .LessThan(1000000).WithMessage("La cantidad de stock no puede exceder 1,000,000.");

            RuleFor(x => x.Categoria)
                .NotEmpty().WithMessage("La categoría es obligatoria.")
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres.");

            RuleFor(x => x.UrlImagen)
                .MaximumLength(500).WithMessage("La URL de la imagen no puede exceder 500 caracteres.")
                .Must(SerUrlSegura).WithMessage("La URL de la imagen debe comenzar con http:// o https://.");

            RuleFor(x => x.CodigoSKU)
                .NotEmpty().WithMessage("El código SKU es obligatorio.")
                .Matches("^[A-Za-z0-9-]{3,50}$").WithMessage("El código SKU solo puede contener letras, números y guiones, entre 3 y 50 caracteres.");
        }

        private bool NoContenerCaracteresEspeciales(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            var caracteresProhibidos = new[] { "<", ">", ";", "--", "/*", "*/", "xp_", "sp_", "DROP", "DELETE", "INSERT", "UPDATE", "EXEC", "EXECUTE" };
            return !caracteresProhibidos.Any(c => texto.Contains(c, StringComparison.OrdinalIgnoreCase));
        }

        private bool SerUrlSegura(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true; // Permitir vacío si no es obligatorio
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }
    }

    public class ActualizarProductoRequestValidator : AbstractValidator<ActualizarProductoRequest>
    {
        public ActualizarProductoRequestValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres.");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a cero.")
                .LessThan(10000000).WithMessage("El precio no puede exceder 10,000,000.");

            RuleFor(x => x.Categoria)
                .NotEmpty().WithMessage("La categoría es obligatoria.")
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres.");

            RuleFor(x => x.UrlImagen)
                .MaximumLength(500).WithMessage("La URL de la imagen no puede exceder 500 caracteres.");
        }
    }

    public class AjustarStockRequestValidator : AbstractValidator<AjustarStockRequest>
    {
        public AjustarStockRequestValidator()
        {
            RuleFor(x => x.CantidadAjuste)
                .NotEqual(0).WithMessage("El ajuste no puede ser cero.")
                .GreaterThan(-1000000).WithMessage("El ajuste no puede ser menor a -1,000,000.")
                .LessThan(1000000).WithMessage("El ajuste no puede exceder 1,000,000.");

            RuleFor(x => x.Motivo)
                .NotEmpty().WithMessage("El motivo del ajuste es obligatorio.")
                .MaximumLength(500).WithMessage("El motivo no puede exceder 500 caracteres.");
        }
    }
}

namespace CatalogoProductos.Application.Servicios
{
    using CatalogoProductos.Application.Comandos;
    using CatalogoProductos.Application.Contratos;
    using CatalogoProductos.Application.Consultas;
    using CatalogoProductos.Application.Validadores;

    public interface IServicioGestionProductos
    {
        Task<ProductoResponse> CrearProductoAsync(CrearProductoCommand comando, CancellationToken cancellationToken);
        Task<ProductoResponse> ActualizarProductoAsync(ActualizarProductoCommand comando, CancellationToken cancellationToken);
        Task EliminarProductoAsync(EliminarProductoCommand comando, CancellationToken cancellationToken);
        Task<ProductoResponse> ObtenerProductoPorIdAsync(ObtenerProductoPorIdQuery consulta, CancellationToken cancellationToken);
        Task<RespuestaPaginada<ProductoResponse>> ObtenerProductosPaginadosAsync(ObtenerProductosPaginadosQuery consulta, CancellationToken cancellationToken);
    }

    public interface IServicioGestionStock
    {
        Task<ProductoResponse> AjustarStockAsync(AjustarStockCommand comando, CancellationToken cancellationToken);
    }

    public class ServicioGestionProductos : IServicioGestionProductos
    {
        private readonly IRepositorioProducto _repositorioProducto;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IMapper _mapper;

        public ServicioGestionProductos(
            IRepositorioProducto repositorioProducto,
            IUnidadDeTrabajo unidadDeTrabajo,
            IMapper mapper)
        {
            _repositorioProducto = repositorioProducto;
            _unidadDeTrabajo = unidadDeTrabajo;
            _mapper = mapper;
        }

        public async Task<ProductoResponse> CrearProductoAsync(CrearProductoCommand comando, CancellationToken cancellationToken)
        {
            await new CrearProductoRequestValidator().ValidateAndThrowAsync(comando.Solicitud, cancellationToken);

            if (await _repositorioProducto.ExistePorCodigoSkuAsync(comando.Solicitud.CodigoSKU, cancellationToken))
            {
                throw new SkuDuplicadoException(comando.Solicitud.CodigoSKU);
            }

            var producto = new Producto(
                comando.Solicitud.Nombre,
                comando.Solicitud.Descripcion,
                comando.Solicitud.Precio,
                comando.Solicitud.CantidadStock,
                comando.Solicitud.Categoria,
                comando.Solicitud.UrlImagen,
                comando.Solicitud.CodigoSKU);

            await _repositorioProducto.CrearAsync(producto, cancellationToken);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

            return _mapper.Map<ProductoResponse>(producto);
        }

        public async Task<ProductoResponse> ActualizarProductoAsync(ActualizarProductoCommand comando, CancellationToken cancellationToken)
        {
            await new ActualizarProductoRequestValidator().ValidateAndThrowAsync(comando.Solicitud, cancellationToken);
            var producto = await _repositorioProducto.ObtenerPorIdAsync(comando.IdProducto, cancellationToken)
                ?? throw new ProductoNoEncontradoException(comando.IdProducto);

            producto.ActualizarDatos(
                comando.Solicitud.Nombre,
                comando.Solicitud.Descripcion,
                comando.Solicitud.Precio,
                comando.Solicitud.Categoria,
                comando.Solicitud.UrlImagen);

            await _repositorioProducto.ActualizarAsync(producto, cancellationToken);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
            return _mapper.Map<ProductoResponse>(producto);
        }

        public async Task EliminarProductoAsync(EliminarProductoCommand comando, CancellationToken cancellationToken)
        {
            var producto = await _repositorioProducto.ObtenerPorIdAsync(comando.IdProducto, cancellationToken)
                ?? throw new ProductoNoEncontradoException(comando.IdProducto);

            producto.MarcarComoInactivo();
            await _repositorioProducto.EliminarAsync(producto, cancellationToken);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        }

        public async Task<ProductoResponse> ObtenerProductoPorIdAsync(ObtenerProductoPorIdQuery consulta, CancellationToken cancellationToken)
        {
            var producto = await _repositorioProducto.ObtenerPorIdAsync(consulta.IdProducto, cancellationToken)
                ?? throw new ProductoNoEncontradoException(consulta.IdProducto);
            return _mapper.Map<ProductoResponse>(producto);
        }

        public async Task<RespuestaPaginada<ProductoResponse>> ObtenerProductosPaginadosAsync(ObtenerProductosPaginadosQuery consulta, CancellationToken cancellationToken)
        {
            var resultado = await _repositorioProducto.ObtenerPaginadoAsync(consulta.NumeroPagina, consulta.TamanoPagina, cancellationToken);
            return new RespuestaPaginada<ProductoResponse>
            {
                Elementos = resultado.Elementos.Select(_mapper.Map<ProductoResponse>).ToArray(),
                NumeroPagina = resultado.NumeroPagina,
                TamanoPagina = resultado.TamanoPagina,
                TotalRegistros = resultado.TotalRegistros
            };
        }
    }

    public class ServicioGestionStock : IServicioGestionStock
    {
        private readonly IRepositorioProducto _repositorioProducto;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IMapper _mapper;

        public ServicioGestionStock(
            IRepositorioProducto repositorioProducto,
            IUnidadDeTrabajo unidadDeTrabajo,
            IMapper mapper)
        {
            _repositorioProducto = repositorioProducto;
            _unidadDeTrabajo = unidadDeTrabajo;
            _mapper = mapper;
        }

        public async Task<ProductoResponse> AjustarStockAsync(AjustarStockCommand comando, CancellationToken cancellationToken)
        {
            await new AjustarStockRequestValidator().ValidateAndThrowAsync(comando.Solicitud, cancellationToken);
            var producto = await _repositorioProducto.ObtenerPorIdAsync(comando.IdProducto, cancellationToken)
                ?? throw new ProductoNoEncontradoException(comando.IdProducto);

            producto.AjustarStock(comando.Solicitud.CantidadAjuste);
            await _repositorioProducto.ActualizarAsync(producto, cancellationToken);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
            return _mapper.Map<ProductoResponse>(producto);
        }
    }

    public class MapeoProductoProfile : Profile
    {
        public MapeoProductoProfile()
        {
            CreateMap<Producto, ProductoResponse>();
        }
    }
}
