using CatalogoProductos.Application.Servicios;
using CatalogoProductos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
