using System.Net;
using System.Net.Mail;
using HuTest.Models.Entities;
using Microsoft.Extensions.Options;

namespace HuTest.Services;

/// <summary>
/// Implementación de <see cref="INotificationService"/> vía correo (RN-4 / N2).
/// Si el SMTP no está habilitado (entorno de desarrollo), registra el correo en el log
/// en lugar de enviarlo, de modo que el flujo sea verificable sin un servidor real.
/// </summary>
public sealed class EmailNotificationService(
    IOptions<SmtpOptions> smtpOptions,
    ILogger<EmailNotificationService> logger) : INotificationService
{
    private readonly SmtpOptions _smtp = smtpOptions.Value;

    public async Task EnviarCuentaBloqueadaAsync(Usuario usuario, DateTime finBloqueo, CancellationToken ct = default)
    {
        var asunto = "CEPLAN — Cuenta bloqueada temporalmente";
        var cuerpo =
            $"Estimado(a) {usuario.NombreCompleto},\n\n" +
            "Su cuenta ha sido bloqueada temporalmente debido a múltiples intentos fallidos de inicio de sesión.\n" +
            $"Podrá volver a intentarlo a partir de las {finBloqueo.ToLocalTime():HH:mm} horas.\n\n" +
            "Si usted no realizó estos intentos, contacte con el área de soporte.\n\nCEPLAN";

        if (!_smtp.Habilitado || string.IsNullOrWhiteSpace(_smtp.Host) || string.IsNullOrWhiteSpace(usuario.Email))
        {
            logger.LogInformation(
                "[N2] (mock) Correo de cuenta bloqueada para {Email}. Desbloqueo: {Fin}.\n{Cuerpo}",
                usuario.Email, finBloqueo, cuerpo);
            return;
        }

        await EnviarAsync(usuario.Email!, asunto, cuerpo, ct);
        logger.LogInformation("[N2] Correo de cuenta bloqueada enviado a {Email}.", usuario.Email);
    }

    public async Task EnviarEnlaceActivacionAsync(Usuario usuario, string urlActivacion, CancellationToken ct = default)
    {
        var asunto = "CEPLAN — Active su cuenta";
        var cuerpo =
            $"Estimado(a) {usuario.NombreCompleto},\n\n" +
            "Se ha creado una cuenta para usted en el aplicativo CEPLAN.\n" +
            "Para activarla y poder iniciar sesión, ingrese al siguiente enlace:\n\n" +
            $"{urlActivacion}\n\n" +
            "El enlace es de un solo uso y caduca en 7 días.\n" +
            "Si usted no solicitó esta cuenta, ignore este mensaje.\n\nCEPLAN";

        if (!_smtp.Habilitado || string.IsNullOrWhiteSpace(_smtp.Host) || string.IsNullOrWhiteSpace(usuario.Email))
        {
            logger.LogInformation(
                "[HU-4] (mock) Correo de activación para {Email}. Enlace: {Url}.\n{Cuerpo}",
                usuario.Email, urlActivacion, cuerpo);
            return;
        }

        await EnviarAsync(usuario.Email!, asunto, cuerpo, ct);
        logger.LogInformation("[HU-4] Correo de activación enviado a {Email}.", usuario.Email);
    }

    private async Task EnviarAsync(string destino, string asunto, string cuerpo, CancellationToken ct)
    {
        using var mensaje = new MailMessage(_smtp.From, destino, asunto, cuerpo);
        using var cliente = new SmtpClient(_smtp.Host, _smtp.Port) { EnableSsl = true };

        // Autenticación solo si se configuraron credenciales (secretos vía .env).
        if (!string.IsNullOrWhiteSpace(_smtp.Username))
            cliente.Credentials = new NetworkCredential(_smtp.Username, _smtp.Password);

        await cliente.SendMailAsync(mensaje, ct);
    }
}

/// <summary>Configuración SMTP (sección "Smtp" de appsettings).</summary>
public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string From { get; init; } = "no-reply@ceplan.gob.pe";
    public bool Habilitado { get; init; }

    /// <summary>Usuario de autenticación SMTP (secreto — se provee vía .env / variables de entorno).</summary>
    public string? Username { get; init; }

    /// <summary>Contraseña de autenticación SMTP (secreto — se provee vía .env / variables de entorno).</summary>
    public string? Password { get; init; }
}
