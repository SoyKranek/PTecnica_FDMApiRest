namespace CatalogoProductos.API.Models;

public class RespuestaError
{
    public int CodigoEstado { get; set; }
    public required string CodigoError { get; set; }
    public required string Mensaje { get; set; }
    public string? Detalles { get; set; }
    public List<ErrorValidacion>? Errores { get; set; }
    public string? Sugerencia { get; set; }
    public DateTime MarcaTiempo { get; set; }
    public required string RutaSolicitud { get; set; }
}

public class ErrorValidacion
{
    public required string Campo { get; set; }
    public required string Mensaje { get; set; }
}
