using HuTest.Data;
using HuTest.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HuTest.Services;

public sealed class RegistrationService(
    AppDbContext db,
    IPasswordHasher<Usuario> hasher,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    public async Task<RegistroResultado> CrearAsync(
        string documento,
        string tipoDocumento,
        string nombreCompleto,
        string email,
        string password,
        DatosPerfil? perfil = null,
        CancellationToken ct = default)
    {
        // RN: el par (Documento, TipoDocumento) es único (índice en AppDbContext).
        var existe = await db.Usuarios
            .AnyAsync(u => u.Documento == documento && u.TipoDocumento == tipoDocumento, ct);

        if (existe)
            return new RegistroResultado(RegistroEstado.DocumentoDuplicado, null, null);

        var p = perfil ?? DatosPerfil.Vacio;
        var usuario = new Usuario
        {
            Documento = documento,
            TipoDocumento = tipoDocumento,
            NombreCompleto = nombreCompleto,
            Email = email,
            // Rol se asigna por defecto ("Operador") desde la entidad Usuario.
            Estado = EstadoUsuario.PendienteActivacion, // RN-8: la cuenta nace inactiva.
            // Datos de perfil opcionales (null si no se informaron en el registro).
            Cargo = p.Cargo,
            Entidad = p.Entidad,
            FechaNacimiento = p.FechaNacimiento,
            Nacionalidad = p.Nacionalidad,
            Sexo = p.Sexo,
            CorreoSecundario = p.CorreoSecundario,
            TelefonoMovil = p.TelefonoMovil,
            TelefonoSecundario = p.TelefonoSecundario,
            TelefonoSecundarioTipo = p.TelefonoSecundarioTipo,
            TipoContratacion = p.TipoContratacion,
            FechaContratacion = p.FechaContratacion,
        };
        usuario.PasswordHash = hasher.HashPassword(usuario, password);

        var token = new TokenActivacion(); // Token GUID y expiración a 7 días por defecto.
        usuario.TokensActivacion.Add(token);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Cuenta {Doc} creada en estado PendienteActivacion.", usuario.Documento);
        return new RegistroResultado(RegistroEstado.Exito, usuario, token.Token);
    }
}
