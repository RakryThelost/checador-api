namespace Checador.Api.Models;

public class Empleado
{
    public int Id { get; set; }
    public string NumeroEmpleado { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;

    public List<Checada> Checadas { get; set; } = [];
}