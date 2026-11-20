using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checador.Api.Migrations
{
    /// <inheritdoc />
    public partial class SpReporteHoras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql("""
        CREATE OR ALTER PROCEDURE dbo.sp_ReporteHoras
            @Desde DATETIME2,
            @Hasta DATETIME2
        AS
        BEGIN
            SET NOCOUNT ON;

            WITH Ordenadas AS (
                SELECT c.EmpleadoId, c.Tipo, c.FechaHora,
                       LEAD(c.Tipo)      OVER (PARTITION BY c.EmpleadoId ORDER BY c.FechaHora) AS SigTipo,
                       LEAD(c.FechaHora) OVER (PARTITION BY c.EmpleadoId ORDER BY c.FechaHora) AS SigFecha
                FROM Checadas c
                WHERE c.FechaHora >= @Desde AND c.FechaHora < @Hasta
            )
            SELECT e.Id AS EmpleadoId,
                   e.NumeroEmpleado,
                   e.Nombre,
                   CAST(ISNULL(SUM(DATEDIFF(MINUTE, o.FechaHora, o.SigFecha)), 0) / 60.0 AS DECIMAL(10,2)) AS HorasTrabajadas
            FROM Empleados e
            LEFT JOIN Ordenadas o
                   ON o.EmpleadoId = e.Id AND o.Tipo = 'Entrada' AND o.SigTipo = 'Salida'
            GROUP BY e.Id, e.NumeroEmpleado, e.Nombre
            ORDER BY e.Nombre;
        END
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_ReporteHoras;");
        }
    }
}
