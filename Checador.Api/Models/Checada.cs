namespace Checador.Api.Models;

public enum TipoChecada { Entrada, Salida }

public class Checada
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public TipoChecada Tipo { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    public Empleado? Empleado { get; set; }
}