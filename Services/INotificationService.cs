using HuTest.Models.Entities;

namespace HuTest.Services;

/// <summary>Envío de notificaciones al usuario (RN-4, N2).</summary>
public interface INotificationService
{
    /// <summary>Notifica al usuario que su cuenta fue bloqueada temporalmente (N2).</summary>
    Task EnviarCuentaBloqueadaAsync(Usuario usuario, DateTime finBloqueo, CancellationToken ct = default);

    /// <summary>Envía al usuario recién creado el enlace para activar su cuenta (HU-4).</summary>
    Task EnviarEnlaceActivacionAsync(Usuario usuario, string urlActivacion, CancellationToken ct = default);
}
