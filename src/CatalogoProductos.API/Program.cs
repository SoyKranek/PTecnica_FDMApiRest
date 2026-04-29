using CatalogoProductos.Application.Servicios;
using CatalogoProductos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Catálogo de Productos",
        Version = "v1",
        Description = "API REST para gestión de catálogo de productos y control de inventario",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "soporte@catalogoproductos.com"
        }
    });

    var archivoXml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);
    opciones.IncludeXmlComments(rutaXml);
});

builder.Services.AddAutoMapper(config => config.AddProfile<MapeoProductoProfile>());
builder.Services.AgregarInfraestructura(builder.Configuration);
builder.Services.AddHealthChecks();

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("cors-abierto", politica =>
    {
        politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<CatalogoProductos.API.Middleware.ManejadorExcepcionesGlobalMiddleware>();
app.UseCors("cors-abierto");

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
