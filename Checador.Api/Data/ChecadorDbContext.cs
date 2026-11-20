using Checador.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checador.Api.Data;

public class ChecadorDbContext(DbContextOptions<ChecadorDbContext> options) : DbContext(options)
{

    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Checada> Checadas => Set<Checada>();

    // Precisión por defecto para todos los decimal (incluye ReporteHorasDto del stored procedure)
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(10, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Empleado>(
        e =>
        {
            e.HasIndex(x => x.NumeroEmpleado).IsUnique();
            e.Property(x => x.NumeroEmpleado).HasMaxLength(150).IsRequired();
            e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
        }
       );

       modelBuilder.Entity<Checada>(
        c =>
        {
            c.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(10);
            c.HasIndex(x => new { x.EmpleadoId, x.FechaHora });
            c.HasOne(x => x.Empleado)
             .WithMany(e => e.Checadas)
             .HasForeignKey(x => x.EmpleadoId)
             .OnDelete(DeleteBehavior.Restrict);
        }
       ); 
    }
}