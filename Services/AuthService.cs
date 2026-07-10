using HuTest.Data;
using HuTest.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HuTest.Services;

public sealed class AuthService(
    AppDbContext db,
    IAccountLockService bloqueo,
    IPasswordHasher<Usuario> hasher) : IAuthService
{
    public async Task<LoginResultado> LoginAsync(
        string documento, string tipoDocumento, string password, CancellationToken ct = default)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Documento == documento && u.TipoDocumento == tipoDocumento, ct);

        // Usuario inexistente: no se revela; se trata como credenciales inválidas (sin CVF que incrementar).
        if (usuario is null)
            return new LoginResultado(LoginEstado.CredencialesInvalidas, null, null);

        // RN-5: una cuenta bloqueada no puede iniciar sesión aunque las credenciales sean correctas.
        if (await bloqueo.EstaBloqueadaAsync(usuario, ct))
        {
            var fin = await bloqueo.ObtenerFinBloqueoAsync(usuario, ct);
            return new LoginResultado(LoginEstado.CuentaBloqueada, usuario, fin);
        }

        // RN-1: validación de credenciales.
        var verificacion = hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password);
        if (verificacion == PasswordVerificationResult.Failed)
        {
            await bloqueo.RegistrarIntentoFallidoAsync(usuario, ct); // RN-2/RN-3

            if (usuario.Estado == EstadoUsuario.Bloqueado)
            {
                var fin = await bloqueo.ObtenerFinBloqueoAsync(usuario, ct);
                return new LoginResultado(LoginEstado.CuentaBloqueada, usuario, fin);
            }

            return new LoginResultado(LoginEstado.CredencialesInvalidas, usuario, null);
        }

        // RN-8: la cuenta debe estar activada.
        if (usuario.Estado == EstadoUsuario.PendienteActivacion)
            return new LoginResultado(LoginEstado.PendienteActivacion, usuario, null);

        // RN-7: login exitoso reinicia el CVF.
        await bloqueo.RegistrarLoginExitosoAsync(usuario, ct);
        return new LoginResultado(LoginEstado.Exito, usuario, null);
    }
}
