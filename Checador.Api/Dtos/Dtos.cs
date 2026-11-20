namespace Checador.Api.Dtos;

public record CrearEmpleadoDto(string NumeroEmpleado, string Nombre);
public record EmpleadoDto(int Id, string NumeroEmpleado, string Nombre, bool Activo);
public record RegistrarChecadaDto(string NumeroEmpleado);
public record ChecadaDto(int Id, string NumeroEmpleado, string Tipo,DateTime FechaHora);

public class ReporteHorasDto
{
    public int EmpleadoId { get; set; }
    public string NumeroEmpleado { get; set; } = String.Empty;
    public string Nombre { get; set; } = String.Empty;
    public decimal HorasTrabajadas { get; set; }
}