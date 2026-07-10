namespace HuTest.Services;

/// <summary>Activación de cuentas pendientes mediante token (HU-4, RN-8).</summary>
public interface IActivationService
{
    /// <summary>Valida el token y, si corresponde, activa la cuenta asociada.</summary>
    Task<ActivacionResultado> ActivarPorTokenAsync(string token, CancellationToken ct = default);
}
