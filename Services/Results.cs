using HuTest.Models.Entities;

namespace HuTest.Services;

/// <summary>Resultado posible de un intento de inicio de sesión (HU-1, HU-2, HU-4).</summary>
public enum LoginEstado
{
    Exito,
    CredencialesInvalidas,
    CuentaBloqueada,
    PendienteActivacion
}

/// <summary>Resultado de <see cref="IAuthService.LoginAsync"/>.</summary>
/// <param name="Estado">Desenlace del intento.</param>
/// <param name="Usuario">Usuario autenticado (solo en <see cref="LoginEstado.Exito"/>).</param>
/// <param name="FinBloqueo">Momento de desbloqueo (solo en <see cref="LoginEstado.CuentaBloqueada"/>).</param>
public sealed record LoginResultado(LoginEstado Estado, Usuario? Usuario, DateTime? FinBloqueo);

/// <summary>Resultado posible de la activación de cuenta (HU-4).</summary>
public enum ActivacionEstado
{
    Exito,
    TokenInvalido,
    TokenExpirado,
    YaActivada
}

/// <summary>Resultado de <see cref="IActivationService.ActivarPorTokenAsync"/>.</summary>
public sealed record ActivacionResultado(ActivacionEstado Estado, string? NombreUsuario);

/// <summary>Resultado posible de la creación de una cuenta (registro).</summary>
public enum RegistroEstado
{
    Exito,
    DocumentoDuplicado
}

/// <summary>Resultado de <see cref="IRegistrationService.CrearAsync"/>.</summary>
/// <param name="Estado">Desenlace de la creación.</param>
/// <param name="Usuario">Usuario creado (solo en <see cref="RegistroEstado.Exito"/>).</param>
/// <param name="Token">Token de activación de un solo uso (solo en <see cref="RegistroEstado.Exito"/>).</param>
public sealed record RegistroResultado(RegistroEstado Estado, Usuario? Usuario, string? Token);

/// <summary>
/// Datos de perfil opcionales que acompañan al alta de una cuenta. Todos admiten <c>null</c>:
/// lo que no se informe se guarda vacío y el detalle de perfil lo muestra sin valor.
/// </summary>
public sealed record DatosPerfil(
    string? Cargo = null,
    string? Entidad = null,
    DateOnly? FechaNacimiento = null,
    string? Nacionalidad = null,
    string? Sexo = null,
    string? CorreoSecundario = null,
    string? TelefonoMovil = null,
    string? TelefonoSecundario = null,
    string? TelefonoSecundarioTipo = null,
    string? TipoContratacion = null,
    DateOnly? FechaContratacion = null)
{
    public static readonly DatosPerfil Vacio = new();
}
