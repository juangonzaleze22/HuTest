namespace HuTest.Models.Entities;

/// <summary>
/// Token de un solo uso para activar una cuenta pendiente (RN-8 / HU-4).
/// </summary>
public sealed class TokenActivacion
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    /// <summary>Valor del token (se entrega al usuario, p. ej. por correo).</summary>
    public string Token { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime FechaExpira { get; set; } = DateTime.UtcNow.AddDays(7);

    public bool Usado { get; set; }
}
