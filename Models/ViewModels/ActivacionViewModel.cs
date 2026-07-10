using HuTest.Services;

namespace HuTest.Models.ViewModels;

/// <summary>Resultado de la pantalla de activación de cuenta (HU-4).</summary>
public sealed class ActivacionViewModel
{
    public ActivacionEstado Estado { get; set; }
    public string? NombreUsuario { get; set; }

    public bool EsExito => Estado is ActivacionEstado.Exito or ActivacionEstado.YaActivada;

    public string Titulo => Estado switch
    {
        ActivacionEstado.Exito => $"¡Bienvenido/a, {PrimerNombre}!",
        ActivacionEstado.YaActivada => $"¡Hola de nuevo, {PrimerNombre}!",
        ActivacionEstado.TokenExpirado => "El enlace de activación expiró",
        _ => "Enlace de activación no válido"
    };

    public string Mensaje => Estado switch
    {
        ActivacionEstado.Exito => "Su cuenta en el aplicativo X está activada. Ya puede iniciar sesión.",
        ActivacionEstado.YaActivada => "Su cuenta ya se encontraba activada. Ya puede iniciar sesión.",
        ActivacionEstado.TokenExpirado => "Solicite un nuevo enlace de activación al área de soporte.",
        _ => "El enlace de activación no es válido. Verifique el enlace o contacte con soporte."
    };

    private string PrimerNombre
    {
        get
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario)) return "usuario";
            // "Apellido Apellido, Nombre" -> "Nombre"; si no hay coma, primera palabra.
            var coma = NombreUsuario.IndexOf(',');
            return coma >= 0
                ? NombreUsuario[(coma + 1)..].Trim().Split(' ')[0]
                : NombreUsuario.Split(' ')[0];
        }
    }
}
