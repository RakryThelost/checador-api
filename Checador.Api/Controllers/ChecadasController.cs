using Checador.Api.Data;
using Checador.Api.Dtos;
using Checador.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Checador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChecadasController(ChecadorDbContext db) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<ChecadaDto>> Registrar(RegistrarChecadaDto dto)
    {
        var empleado = await db.Empleados.FirstOrDefaultAsync(e => e.NumeroEmpleado == dto.NumeroEmpleado);
        if (empleado is null) return NotFound($"No se encontró un empleado con el número {dto.NumeroEmpleado}");
        if (!empleado.Activo) return BadRequest($"El empleado {empleado.Nombre} está desactivado");

        var ultima = await db.Checadas
            .Where(c => c.EmpleadoId == empleado.Id)
            .OrderByDescending(c => c.FechaHora)
            .FirstOrDefaultAsync();
        
        var tipo = ultima?.Tipo == TipoChecada.Entrada ? TipoChecada.Salida : TipoChecada.Entrada;
        var checada = new Checada { EmpleadoId = empleado.Id, Tipo = tipo };
        db.Checadas.Add(checada);
        await db.SaveChangesAsync();

        return Ok(new ChecadaDto(checada.Id, empleado.NumeroEmpleado, tipo.ToString(), checada.FechaHora));
    }

    [HttpGet("empleado/{numeroEmpleado}")]
    public async Task<ActionResult<List<ChecadaDto>>> PorEmpleado(string numeroEmpleado, DateTime? desde, DateTime? hasta)
    {
        var query = db.Checadas.AsNoTracking()
            .Where(c => c.Empleado!.NumeroEmpleado == numeroEmpleado);

        if(desde is not null) query = query.Where(c => c.FechaHora >= desde);
        if(hasta is not null) query = query.Where(c => c.FechaHora < hasta);

        return await query
            .OrderBy(c => c.FechaHora)
            .Select(c => new ChecadaDto(c.Id, numeroEmpleado, c.Tipo.ToString(), c.FechaHora))
            .ToListAsync();
    }

    [HttpGet("reporte-horas")]
    public async Task<ActionResult<List<ReporteHorasDto>>> ReporteHoras(DateTime desde, DateTime hasta) => 
        await db.Database
            .SqlQuery<ReporteHorasDto>($"EXEC dbo.sp_ReporteHoras @Desde={desde}, @Hasta={hasta}")
            .ToListAsync();

}