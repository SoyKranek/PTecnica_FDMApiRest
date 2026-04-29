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
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Precio).GreaterThan(0);
            RuleFor(x => x.CantidadStock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Categoria).NotEmpty().MaximumLength(100);
            RuleFor(x => x.UrlImagen).MaximumLength(500);
            RuleFor(x => x.CodigoSKU).NotEmpty().Matches("^[A-Za-z0-9-]{3,50}$");
        }
    }

    public class ActualizarProductoRequestValidator : AbstractValidator<ActualizarProductoRequest>
    {
        public ActualizarProductoRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Precio).GreaterThan(0);
            RuleFor(x => x.Categoria).NotEmpty().MaximumLength(100);
            RuleFor(x => x.UrlImagen).MaximumLength(500);
        }
    }

    public class AjustarStockRequestValidator : AbstractValidator<AjustarStockRequest>
    {
        public AjustarStockRequestValidator()
        {
            RuleFor(x => x.CantidadAjuste).NotEqual(0);
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
