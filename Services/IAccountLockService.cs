using HuTest.Models.Entities;

namespace HuTest.Services;

/// <summary>
/// Gestiona el contador de validaciones fallidas (CVF) y el bloqueo temporal de cuentas
/// (RN-2, RN-3, RN-5, RN-6, RN-7).
/// </summary>
public interface IAccountLockService
{
    /// <summary>
    /// Indica si la cuenta está bloqueada en este momento. Aplica desbloqueo perezoso:
    /// si el bloqueo ya venció, lo levanta y reinicia el CVF (RN-6).
    /// </summary>
    Task<bool> EstaBloqueadaAsync(Usuario usuario, CancellationToken ct = default);

    /// <summary>Registra un intento fallido; incrementa el CVF y bloquea si alcanza el umbral (RN-2/RN-3/RN-4).</summary>
    Task RegistrarIntentoFallidoAsync(Usuario usuario, CancellationToken ct = default);

    /// <summary>Reinicia el CVF tras un inicio de sesión exitoso (RN-7).</summary>
    Task RegistrarLoginExitosoAsync(Usuario usuario, CancellationToken ct = default);

    /// <summary>Devuelve el momento de desbloqueo del bloqueo activo, o null si no hay bloqueo vigente.</summary>
    Task<DateTime?> ObtenerFinBloqueoAsync(Usuario usuario, CancellationToken ct = default);
}
