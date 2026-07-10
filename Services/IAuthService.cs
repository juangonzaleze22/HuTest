namespace HuTest.Services;

/// <summary>Valida credenciales y determina el desenlace del login (HU-1, RN-1/RN-5/RN-8).</summary>
public interface IAuthService
{
    /// <summary>
    /// Valida las credenciales aplicando las reglas de bloqueo, activación y CVF.
    /// No emite la cookie de autenticación; eso es responsabilidad del controlador.
    /// </summary>
    Task<LoginResultado> LoginAsync(string documento, string tipoDocumento, string password, CancellationToken ct = default);
}
