using System.ComponentModel.DataAnnotations;

namespace HuTest.Models.Entities;

/// <summary>
/// Cuenta de usuario del sistema CEPLAN. Concentra las credenciales, el estado de la cuenta
/// y el contador de validaciones fallidas (CVF). Ver spec.md §2 y reglas RN-1..RN-8.
/// </summary>
public sealed class Usuario
{
    public int Id { get; set; }

    /// <summary>Documento de identidad usado como usuario (DNI o CE).</summary>
    [Required, MaxLength(20)]
    public string Documento { get; set; } = string.Empty;

    /// <summary>Tipo de documento con el que se autentica: "DNI" o "CE".</summary>
    [Required, MaxLength(3)]
    public string TipoDocumento { get; set; } = "DNI";

    [Required, MaxLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? Email { get; set; }

    [MaxLength(80)]
    public string Rol { get; set; } = "Operador";

    /// <summary>Hash de la contraseña (nunca se almacena en claro).</summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public EstadoUsuario Estado { get; set; } = EstadoUsuario.PendienteActivacion;

    // ---- Datos de perfil (opcionales) ----
    // Se capturan en el registro o se completan más tarde. Si no se informan, el detalle de
    // perfil los muestra vacíos (ver PerfilController). Reemplazan a los valores fijos que antes
    // se "quemaban" en el ViewModel.

    [MaxLength(120)]
    public string? Cargo { get; set; }

    [MaxLength(150)]
    public string? Entidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    [MaxLength(60)]
    public string? Nacionalidad { get; set; }

    [MaxLength(20)]
    public string? Sexo { get; set; }

    [MaxLength(256)]
    public string? CorreoSecundario { get; set; }

    [MaxLength(30)]
    public string? TelefonoMovil { get; set; }

    [MaxLength(30)]
    public string? TelefonoSecundario { get; set; }

    /// <summary>Tipo del teléfono secundario: "Fijo" o "Móvil".</summary>
    [MaxLength(10)]
    public string? TelefonoSecundarioTipo { get; set; }

    [MaxLength(40)]
    public string? TipoContratacion { get; set; }

    public DateOnly? FechaContratacion { get; set; }

    /// <summary>RN-2: contador de validaciones fallidas acumuladas.</summary>
    public int Cvf { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    public ICollection<BloqueoCuenta> Bloqueos { get; set; } = new List<BloqueoCuenta>();
    public ICollection<TokenActivacion> TokensActivacion { get; set; } = new List<TokenActivacion>();
}
