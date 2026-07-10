using System.ComponentModel.DataAnnotations;

namespace HuTest.Models.ViewModels;

/// <summary>
/// Datos del formulario de creación de cuentas (registro). Sigue el patrón visual del login
/// y da de alta un <see cref="Entities.Usuario"/> en estado PendienteActivacion (RN-8 / HU-4).
/// </summary>
public sealed class RegistroViewModel
{
    /// <summary>Tipo de documento seleccionado en el toggle: "DNI" o "CE".</summary>
    [Required]
    public string TipoDocumento { get; set; } = "DNI";

    [Required(ErrorMessage = "Ingrese el número de documento.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "El documento debe tener entre 8 y 20 caracteres.")]
    [Display(Name = "Documento")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese el nombre completo.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese el correo electrónico.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(256)]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese una contraseña.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;

    // ---- Datos de perfil (opcionales) ----
    // Sin [Required]: si el usuario los deja en blanco se guardan como null y el perfil los
    // muestra vacíos.

    [StringLength(120)]
    [Display(Name = "Cargo")]
    public string? Cargo { get; set; }

    [StringLength(150)]
    [Display(Name = "Entidad")]
    public string? Entidad { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [StringLength(60)]
    [Display(Name = "Nacionalidad")]
    public string? Nacionalidad { get; set; }

    [StringLength(20)]
    [Display(Name = "Sexo")]
    public string? Sexo { get; set; }

    [EmailAddress(ErrorMessage = "El correo secundario no tiene un formato válido.")]
    [StringLength(256)]
    [Display(Name = "Correo electrónico secundario")]
    public string? CorreoSecundario { get; set; }

    [StringLength(30)]
    [Display(Name = "Teléfono móvil")]
    public string? TelefonoMovil { get; set; }

    [StringLength(30)]
    [Display(Name = "Teléfono secundario")]
    public string? TelefonoSecundario { get; set; }

    [StringLength(10)]
    [Display(Name = "Tipo de teléfono secundario")]
    public string? TelefonoSecundarioTipo { get; set; }

    [StringLength(40)]
    [Display(Name = "Tipo de contratación")]
    public string? TipoContratacion { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de contratación")]
    public DateOnly? FechaContratacion { get; set; }

    // ---- Estado de resultado (se completa tras un registro exitoso) ----

    /// <summary>True cuando la cuenta se creó correctamente; la vista muestra la confirmación.</summary>
    public bool Exito { get; set; }

    /// <summary>Nombre de pila para el saludo de la confirmación.</summary>
    public string? NombreUsuario { get; set; }

    /// <summary>
    /// Enlace de activación generado. En un entorno real viaja solo por correo; aquí se expone
    /// además en pantalla como comodidad de demostración (el SMTP está en modo simulado).
    /// </summary>
    public string? EnlaceActivacion { get; set; }
}
