using CatalogoProductos.Application.Servicios;
using CatalogoProductos.Domain.Compartido;
using CatalogoProductos.Domain.Entidades;
using CatalogoProductos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogoProductos.Infrastructure.Datos
{
    public class ContextoAplicacion : DbContext, IUnidadDeTrabajo
    {
        public ContextoAplicacion(DbContextOptions<ContextoAplicacion> options) : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ConfiguracionProducto());
        }

        Task<int> IUnidadDeTrabajo.GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }

    public class ConfiguracionProducto : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("productos");
            builder.HasKey(x => x.IdProducto);
            builder.Property(x => x.IdProducto).HasColumnName("id_producto");
            builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
            builder.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(2000).IsRequired();
            builder.Property(x => x.Precio).HasColumnName("precio").HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.CantidadStock).HasColumnName("cantidad_stock").IsRequired();
            builder.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(100).IsRequired();
            builder.Property(x => x.UrlImagen).HasColumnName("url_imagen").HasMaxLength(500);
            builder.Property(x => x.CodigoSKU).HasColumnName("codigo_sku").HasMaxLength(50).IsRequired();
            builder.Property(x => x.EstaActivo).HasColumnName("esta_activo").IsRequired();
            builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").IsRequired();
            builder.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");
            builder.HasIndex(x => x.CodigoSKU).IsUnique();
            builder.HasIndex(x => x.Categoria);
            builder.HasIndex(x => x.EstaActivo);
        }
    }
}

namespace CatalogoProductos.Infrastructure.Repositorios
{
    using CatalogoProductos.Infrastructure.Datos;

    public class RepositorioProducto : IRepositorioProducto
    {
        private readonly ContextoAplicacion _contextoAplicacion;

        public RepositorioProducto(ContextoAplicacion contextoAplicacion)
        {
            _contextoAplicacion = contextoAplicacion;
        }

        public async Task<Producto?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken = default)
        {
            return await _contextoAplicacion.Productos.FirstOrDefaultAsync(x => x.IdProducto == idProducto, cancellationToken);
        }

        public async Task<ResultadoPaginado<Producto>> ObtenerPaginadoAsync(
            int numeroPagina,
            int tamanoPagina,
            CancellationToken cancellationToken = default)
        {
            var consulta = _contextoAplicacion.Productos.AsNoTracking().Where(x => x.EstaActivo);
            var totalRegistros = await consulta.CountAsync(cancellationToken);
            var productos = await consulta
                .OrderBy(x => x.Nombre)
                .Skip((numeroPagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync(cancellationToken);

            return new ResultadoPaginado<Producto>
            {
                Elementos = productos,
                NumeroPagina = numeroPagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<bool> ExistePorCodigoSkuAsync(string codigoSku, CancellationToken cancellationToken = default)
        {
            return await _contextoAplicacion.Productos.AnyAsync(x => x.CodigoSKU == codigoSku, cancellationToken);
        }

        public Task CrearAsync(Producto producto, CancellationToken cancellationToken = default)
        {
            _contextoAplicacion.Productos.Add(producto);
            return Task.CompletedTask;
        }

        public Task ActualizarAsync(Producto producto, CancellationToken cancellationToken = default)
        {
            _contextoAplicacion.Productos.Update(producto);
            return Task.CompletedTask;
        }

        public Task EliminarAsync(Producto producto, CancellationToken cancellationToken = default)
        {
            _contextoAplicacion.Productos.Update(producto);
            return Task.CompletedTask;
        }
    }
}

namespace CatalogoProductos.Infrastructure
{
    using CatalogoProductos.Infrastructure.Datos;
    using CatalogoProductos.Infrastructure.Repositorios;

    public static class InyeccionDependenciasInfrastructure
    {
        public static IServiceCollection AgregarInfraestructura(this IServiceCollection services, IConfiguration configuration)
        {
            var cadenaConexion = configuration["DATABASE_URL"]
                ?? configuration.GetConnectionString("PostgreSQL")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Database=catalogoproductos;Username=postgres;Password=postgres";

            services.AddDbContext<ContextoAplicacion>(options => options.UseNpgsql(cadenaConexion));
            services.AddScoped<IRepositorioProducto, RepositorioProducto>();
            services.AddScoped<IUnidadDeTrabajo>(provider => provider.GetRequiredService<ContextoAplicacion>());
            services.AddScoped<IServicioGestionProductos, ServicioGestionProductos>();
            services.AddScoped<IServicioGestionStock, ServicioGestionStock>();

            return services;
        }
    }
}
