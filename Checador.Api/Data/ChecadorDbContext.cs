using Checador.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checador.Api.Data;

public class ChecadorDbContext(DbContextOptions<ChecadorDbContext> options) : DbContext(options)
{

    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Checada> Checadas => Set<Checada>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Empleado>(
        e =>
        {
            e.HasIndex(e => e.NumeroEmpleado).IsUnique();
            e.Property(e => e.NumeroEmpleado).HasMaxLength(150).IsRequired();
            e.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        }
       );

       modelBuilder.Entity<Checada>(
        c =>
        {
            c.Property(c => c.Tipo).HasConversion<string>().HasMaxLength(10);
            c.HasIndex(c => new { c.EmpleadoId, c.FechaHora });
            c.HasOne(c => c.Empleado)
             .WithMany(e => e.Checadas)
             .HasForeignKey(c => c.EmpleadoId)
             .OnDelete(DeleteBehavior.Restrict);
        }
       ); 
    }
}