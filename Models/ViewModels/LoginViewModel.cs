using System.ComponentModel.DataAnnotations;

namespace HuTest.Models.ViewModels;

/// <summary>Datos del formulario de login (HU-1). Refleja la pantalla "Gestión de usuarios" del Figma.</summary>
public sealed class LoginViewModel
{
    /// <summary>Tipo de documento seleccionado en el toggle: "DNI" o "CE".</summary>
    [Required]
    public string TipoDocumento { get; set; } = "DNI";

    [Required(ErrorMessage = "Ingrese su usuario.")]
    [Display(Name = "Usuario")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
