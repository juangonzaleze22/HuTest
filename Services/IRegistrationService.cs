namespace HuTest.Services;

/// <summary>Alta de cuentas de usuario en estado pendiente de activación (HU-4, RN-8).</summary>
public interface IRegistrationService
{
    /// <summary>
    /// Crea un usuario en estado <c>PendienteActivacion</c> y le genera un token de activación
    /// de un solo uso. No envía el correo: eso lo orquesta el controlador (necesita la URL absoluta).
    /// El rol se asigna por defecto ("Operador"): único rol contemplado en la spec.
    /// </summary>
    Task<RegistroResultado> CrearAsync(
        string documento,
        string tipoDocumento,
        string nombreCompleto,
        string email,
        string password,
        DatosPerfil? perfil = null,
        CancellationToken ct = default);
}
