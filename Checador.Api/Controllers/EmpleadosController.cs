using Checador.Api.Data;
using Checador.Api.Dtos;
using Checador.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Checador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController(ChecadorDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EmpleadoDto>>> GetAll() =>
        await db.Empleados.AsNoTracking()
            .OrderBy(e => e.Nombre)
            .Select(e => new EmpleadoDto(e.Id, e.NumeroEmpleado, e.Nombre, e.Activo))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmpleadoDto>> GetById(int id)
    {
        var e = await db.Empleados.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return e is null ? NotFound() : new EmpleadoDto(e.Id, e.NumeroEmpleado, e.Nombre, e.Activo);
    }

    [HttpPost]
    public async Task<ActionResult<EmpleadoDto>> Create(CrearEmpleadoDto dto)
    {
        if (await db.Empleados.AnyAsync(e => e.NumeroEmpleado == dto.NumeroEmpleado))
            return Conflict($"Ya existe un empleado con el número {dto.NumeroEmpleado}");

        var empleado = new Empleado { NumeroEmpleado = dto.NumeroEmpleado, Nombre = dto.Nombre };
        db.Empleados.Add(empleado);
        await db.SaveChangesAsync();

        var result = new EmpleadoDto(empleado.Id, empleado.NumeroEmpleado, empleado.Nombre, empleado.Activo);
        return CreatedAtAction(nameof(GetById), new { id = empleado.Id }, result);
    }

    [HttpPut("{id:int}/desactivar")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var e = await db.Empleados.FindAsync(id);
        if (e is null) return NotFound();
        e.Activo = false;
        await db.SaveChangesAsync();
        return NoContent();
    }
}