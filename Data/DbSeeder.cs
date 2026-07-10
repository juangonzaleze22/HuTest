using HuTest.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HuTest.Data;

/// <summary>
/// Crea la base de datos (si no existe) y siembra datos de prueba para poder ejercitar
/// los flujos de la HU-001 sin un backend de usuarios externo.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher<Usuario> hasher)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.Usuarios.AnyAsync())
            return;

        // Usuario activo de prueba — coincide con el perfil "Osorio Montes, Adriana / Operador" del Figma.
        // (El rol se hereda por defecto de la entidad Usuario: "Operador".)
        var activo = new Usuario
        {
            Documento = "12345678",
            TipoDocumento = "DNI",
            NombreCompleto = "Osorio Montes, Adriana",
            Email = "adriana.osorio@ceplan.gob.pe",
            Estado = EstadoUsuario.Activo,
            // Datos de perfil de ejemplo (solo para el usuario demo del Figma).
            Cargo = "Administrador de Recursos",
            Entidad = "011 Ministerio de Salud",
            FechaNacimiento = new DateOnly(1944, 4, 1),
            Nacionalidad = "Peruana",
            Sexo = "Femenino",
            TelefonoMovil = "+51 999 999 999",
            TipoContratacion = "CAS",
            FechaContratacion = new DateOnly(2015, 3, 9)
        };
        activo.PasswordHash = hasher.HashPassword(activo, "Ceplan2025");

        // Usuario pendiente de activación — con token conocido para probar HU-4.
        var pendiente = new Usuario
        {
            Documento = "87654321",
            TipoDocumento = "DNI",
            NombreCompleto = "Ramírez Vega, July",
            Email = "july.ramirez@ceplan.gob.pe",
            Estado = EstadoUsuario.PendienteActivacion
        };
        pendiente.PasswordHash = hasher.HashPassword(pendiente, "Ceplan2025");
        pendiente.TokensActivacion.Add(new TokenActivacion
        {
            Token = "TOKEN-DEMO-JULY",
            FechaExpira = DateTime.UtcNow.AddDays(7)
        });

        db.Usuarios.AddRange(activo, pendiente);
        await db.SaveChangesAsync();
    }
}
