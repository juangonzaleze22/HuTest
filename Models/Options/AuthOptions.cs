namespace HuTest.Models.Options;

/// <summary>
/// Parámetros de negocio de autenticación y sesión (sección "Auth" de appsettings).
/// Los valores por defecto coinciden con la especificación HU-001 (spec.md §3).
/// </summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public CvfOptions Cvf { get; init; } = new();
    public BloqueoOptions Bloqueo { get; init; } = new();
    public SesionOptions Sesion { get; init; } = new();

    /// <summary>RN-2/RN-3: contador de validaciones fallidas.</summary>
    public sealed class CvfOptions
    {
        /// <summary>Número de intentos fallidos que dispara el bloqueo temporal.</summary>
        public int Umbral { get; init; } = 5;
    }

    /// <summary>RN-6: bloqueo temporal de la cuenta.</summary>
    public sealed class BloqueoOptions
    {
        /// <summary>Duración del bloqueo temporal en minutos.</summary>
        public int Minutos { get; init; } = 15;
    }

    /// <summary>RN-9/RN-10: gestión de sesión por inactividad.</summary>
    public sealed class SesionOptions
    {
        /// <summary>Minutos de inactividad tras los cuales expira la sesión.</summary>
        public int InactividadMinutos { get; init; } = 20;

        /// <summary>Segundos de antelación con los que se muestra el aviso de expiración.</summary>
        public int AvisoSegundos { get; init; } = 49;
    }
}
