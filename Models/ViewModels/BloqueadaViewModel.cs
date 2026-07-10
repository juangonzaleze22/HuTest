namespace HuTest.Models.ViewModels;

/// <summary>Pantalla informativa de cuenta bloqueada temporalmente (HU-2, CA-HU-2.2).</summary>
public sealed class BloqueadaViewModel
{
    public DateTime? FinBloqueo { get; set; }

    /// <summary>Umbral de intentos fallidos que dispara el bloqueo (RN-2, config Auth:Cvf:Umbral).</summary>
    public int Umbral { get; set; }

    /// <summary>Duración total del bloqueo en minutos (RN-6, config Auth:Bloqueo:Minutos).</summary>
    public int DuracionMinutos { get; set; }

    /// <summary>Minutos restantes (redondeados hacia arriba) hasta el desbloqueo.</summary>
    public int MinutosRestantes =>
        FinBloqueo is { } fin && fin > DateTime.UtcNow
            ? (int)Math.Ceiling((fin - DateTime.UtcNow).TotalMinutes)
            : 0;
}
