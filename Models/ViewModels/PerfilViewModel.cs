namespace HuTest.Models.ViewModels;

/// <summary>
/// Datos de presentación de la pantalla «Perfil de usuario» (detalle).
/// Refleja el diseño Figma: cabecera con estado + pestañas + grilla de campos.
/// Los campos opcionales que el usuario no informó en el registro se muestran vacíos
/// (son <c>null</c>): no se rellenan con valores de ejemplo.
/// </summary>
public sealed class PerfilViewModel
{
    // Cabecera
    public string NombreMostrado { get; set; } = string.Empty; // "Apellidos, Nombres"
    public string Iniciales { get; set; } = "U";
    public string Estado { get; set; } = "Activo";
    public bool EstaActivo { get; set; } = true;
    public string? Cargo { get; set; }
    public string? Entidad { get; set; }

    // Información básica
    public string Nombres { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string SegundoApellido { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = "DNI";
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? FechaNacimiento { get; set; }
    public string? Nacionalidad { get; set; }
    public string? Sexo { get; set; }

    public string? CorreoPrincipal { get; set; }
    public string? CorreoSecundario { get; set; }
    public string? TelefonoMovil { get; set; }
    public string? TelefonoSecundario { get; set; }
    public string? TelefonoSecundarioTipo { get; set; }

    public string? TipoContratacion { get; set; }
    public string? FechaContratacion { get; set; }
}
