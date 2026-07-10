namespace HuTest.Models.Entities;

/// <summary>
/// Registro de un bloqueo temporal de cuenta (RN-3/RN-5/RN-6).
/// El desbloqueo es "perezoso": se evalúa comparando <see cref="FechaFin"/> con la hora actual.
/// </summary>
public sealed class BloqueoCuenta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

    /// <summary>Momento en el que el bloqueo deja de tener efecto.</summary>
    public DateTime FechaFin { get; set; }

    /// <summary>False cuando el bloqueo ya fue levantado (expirado).</summary>
    public bool Activo { get; set; } = true;
}
