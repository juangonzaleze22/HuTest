using HuTest.Data;
using HuTest.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HuTest.Services;

public sealed class ActivationService(AppDbContext db, ILogger<ActivationService> logger) : IActivationService
{
    public async Task<ActivacionResultado> ActivarPorTokenAsync(string token, CancellationToken ct = default)
    {
        var registro = await db.TokensActivacion
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Token == token, ct);

        if (registro?.Usuario is null)
            return new ActivacionResultado(ActivacionEstado.TokenInvalido, null);

        var usuario = registro.Usuario;

        if (usuario.Estado == EstadoUsuario.Activo)
            return new ActivacionResultado(ActivacionEstado.YaActivada, usuario.NombreCompleto);

        if (registro.Usado || registro.FechaExpira <= DateTime.UtcNow)
            return new ActivacionResultado(ActivacionEstado.TokenExpirado, usuario.NombreCompleto);

        usuario.Estado = EstadoUsuario.Activo;
        registro.Usado = true;
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Cuenta {Doc} activada mediante token.", usuario.Documento);
        return new ActivacionResultado(ActivacionEstado.Exito, usuario.NombreCompleto);
    }
}
