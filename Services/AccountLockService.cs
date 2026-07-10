using HuTest.Data;
using HuTest.Models.Entities;
using HuTest.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HuTest.Services;

public sealed class AccountLockService(
    AppDbContext db,
    INotificationService notificaciones,
    IOptions<AuthOptions> options,
    ILogger<AccountLockService> logger) : IAccountLockService
{
    private readonly AuthOptions _auth = options.Value;

    public async Task<bool> EstaBloqueadaAsync(Usuario usuario, CancellationToken ct = default)
    {
        var bloqueo = await db.Bloqueos
            .Where(b => b.UsuarioId == usuario.Id && b.Activo)
            .OrderByDescending(b => b.FechaFin)
            .FirstOrDefaultAsync(ct);

        if (bloqueo is null)
            return false;

        if (bloqueo.FechaFin <= DateTime.UtcNow)
        {
            // Desbloqueo perezoso (RN-6): el temporizador venció.
            bloqueo.Activo = false;
            usuario.Estado = EstadoUsuario.Activo;
            usuario.Cvf = 0;
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Cuenta {Doc} desbloqueada automáticamente.", usuario.Documento);
            return false;
        }

        return true;
    }

    public async Task RegistrarIntentoFallidoAsync(Usuario usuario, CancellationToken ct = default)
    {
        usuario.Cvf++;

        if (usuario.Cvf >= _auth.Cvf.Umbral)
        {
            var finBloqueo = DateTime.UtcNow.AddMinutes(_auth.Bloqueo.Minutos);
            usuario.Estado = EstadoUsuario.Bloqueado;
            db.Bloqueos.Add(new BloqueoCuenta
            {
                UsuarioId = usuario.Id,
                FechaInicio = DateTime.UtcNow,
                FechaFin = finBloqueo,
                Activo = true
            });
            await db.SaveChangesAsync(ct);

            logger.LogWarning("Cuenta {Doc} bloqueada por alcanzar CVF={Cvf}.", usuario.Documento, usuario.Cvf);
            await notificaciones.EnviarCuentaBloqueadaAsync(usuario, finBloqueo, ct); // RN-4 (N2)
            return;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task RegistrarLoginExitosoAsync(Usuario usuario, CancellationToken ct = default)
    {
        if (usuario.Cvf != 0)
        {
            usuario.Cvf = 0;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<DateTime?> ObtenerFinBloqueoAsync(Usuario usuario, CancellationToken ct = default)
    {
        var bloqueo = await db.Bloqueos
            .Where(b => b.UsuarioId == usuario.Id && b.Activo && b.FechaFin > DateTime.UtcNow)
            .OrderByDescending(b => b.FechaFin)
            .FirstOrDefaultAsync(ct);

        return bloqueo?.FechaFin;
    }
}
