# Checador API

API REST para control de asistencia (entradas y salidas de empleados), construida con **.NET 10**, **Entity Framework Core 10** y **SQL Server**.

## Funcionalidad
- **Empleados:** alta, consulta y desactivación (número de empleado único).
- **Checadas:** el empleado solo envía su número; el sistema decide si es **Entrada** o **Salida** según su última checada.
- **Reporte de horas trabajadas** por rango de fechas mediante un **stored procedure** en T-SQL (`sp_ReporteHoras`), que empareja cada entrada con su siguiente salida usando la función de ventana `LEAD()`.

## Stack
- ASP.NET Core Web API (controladores) · .NET 10 (LTS)
- Entity Framework Core 10 · migraciones (incluido el stored procedure, versionado como migración)
- SQL Server 2022 en Docker
- OpenAPI + Scalar para documentación y pruebas

## Decisiones de diseño
- **DTOs** separados para entrada y salida (el cliente no envía `Id` ni `Activo`).
- **Índice único** en `NumeroEmpleado` e **índice compuesto** `(EmpleadoId, FechaHora)` para las consultas por empleado y rango de fechas.
- `TipoChecada` guardado como **texto** en la base de datos (legible en consultas directas).
- `DeleteBehavior.Restrict`: no se puede borrar un empleado con historial de checadas.
- Fechas en **UTC**; la conversión a hora local se hace al presentar.
- Consultas parametrizadas (`SqlQuery` con interpolación) para evitar inyección SQL.

## Cómo correrlo
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Checador#2026" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
cd Checador.Api
dotnet ef database update
dotnet run
```
Abrir `http://localhost:5166/scalar` para probar los endpoints.

> La cadena de conexión en `appsettings.Development.json` es solo para desarrollo local. En producción se usaría Azure Key Vault o variables de entorno.

## Endpoints
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Empleados` | Lista de empleados |
| GET | `/api/Empleados/{id}` | Empleado por id |
| POST | `/api/Empleados` | Alta de empleado |
| PUT | `/api/Empleados/{id}/desactivar` | Desactivar empleado |
| POST | `/api/Checadas` | Registrar checada (entrada/salida automática) |
| GET | `/api/Checadas/empleado/{numero}?desde=&hasta=` | Checadas de un empleado |
| GET | `/api/Checadas/reporte-horas?desde=&hasta=` | Horas trabajadas por empleado (stored procedure) |

## Próximos pasos
- Despliegue en Azure App Service + Azure SQL
- Pruebas unitarias con xUnit
- Frontend en Angular
