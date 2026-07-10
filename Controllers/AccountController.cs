using System.Security.Claims;
using HuTest.Models.Entities;
using HuTest.Models.ViewModels;
using HuTest.Services;
using HuTest.Models.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HuTest.Controllers;

public sealed class AccountController(
    IAuthService auth,
    IActivationService activacion,
    IRegistrationService registro,
    INotificationService notificaciones,
    IOptions<AuthOptions> authOptions) : Controller
{
    private readonly AuthOptions _auth = authOptions.Value;

    // ---- HU-1: Login ----

    [HttpGet]
    public IActionResult Login(string? returnUrl = null, bool expirado = false)
    {
        if (expirado)
            TempData["SesionExpirada"] = true; // dispara el toast (CA-HU-6.2)

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resultado = await auth.LoginAsync(model.Documento.Trim(), model.TipoDocumento, model.Password);

        switch (resultado.Estado)
        {
            case LoginEstado.Exito:
                await FirmarSesionAsync(resultado.Usuario!);
                return RedirectToLocal(model.ReturnUrl);

            case LoginEstado.CuentaBloqueada:
                TempData["FinBloqueo"] = resultado.FinBloqueo?.ToString("o");
                return RedirectToAction(nameof(Bloqueada));

            case LoginEstado.PendienteActivacion:
                ModelState.AddModelError(string.Empty,
                    "Su cuenta aún no está activada. Revise su correo de activación o contacte con soporte.");
                return View(model);

            case LoginEstado.CredencialesInvalidas:
            default:
                // Se marcan ambos campos para que el error aparezca bajo cada input
                // (borde rojo + mensaje), en lugar de un aviso general encima del formulario.
                ModelState.AddModelError(nameof(model.Documento), "Usuario incorrecto");
                ModelState.AddModelError(nameof(model.Password), "Contraseña incorrecta");
                return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(bool expirado = false)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login), new { expirado });
    }

    // Logout disparado por el temporizador de inactividad (HU-6). La cookie ya puede
    // haber expirado en el momento del POST, por lo que la petición llega anónima y el
    // token antiforgery (ligado al usuario) no coincidiría: por eso se ignora aquí.
    [HttpPost]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> LogoutExpirado()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login), new { expirado = true });
    }

    // ---- HU-2: Cuenta bloqueada ----

    [HttpGet]
    public IActionResult Bloqueada()
    {
        DateTime? fin = TempData["FinBloqueo"] is string s
            && DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out var d)
                ? d
                : null;
        return View(new BloqueadaViewModel
        {
            FinBloqueo = fin,
            Umbral = _auth.Cvf.Umbral,
            DuracionMinutos = _auth.Bloqueo.Minutos
        });
    }

    // ---- Registro: creación de cuentas ----

    [HttpGet]
    public IActionResult Registrar() => View(new RegistroViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resultado = await registro.CrearAsync(
            model.Documento.Trim(),
            model.TipoDocumento,
            model.NombreCompleto.Trim(),
            model.Email.Trim(),
            model.Password,
            new DatosPerfil(
                Cargo: Limpiar(model.Cargo),
                Entidad: Limpiar(model.Entidad),
                FechaNacimiento: model.FechaNacimiento,
                Nacionalidad: Limpiar(model.Nacionalidad),
                Sexo: Limpiar(model.Sexo),
                CorreoSecundario: Limpiar(model.CorreoSecundario),
                TelefonoMovil: Limpiar(model.TelefonoMovil),
                TelefonoSecundario: Limpiar(model.TelefonoSecundario),
                TelefonoSecundarioTipo: Limpiar(model.TelefonoSecundarioTipo),
                TipoContratacion: Limpiar(model.TipoContratacion),
                FechaContratacion: model.FechaContratacion));

        if (resultado.Estado == RegistroEstado.DocumentoDuplicado)
        {
            ModelState.AddModelError(nameof(model.Documento),
                "Ya existe una cuenta con ese tipo y número de documento.");
            return View(model);
        }

        // Enlace absoluto de activación (HU-4). En un entorno real solo viaja por correo.
        var urlActivacion = Url.Action(
            nameof(Activar), "Account",
            new { token = resultado.Token },
            Request.Scheme)!;

        await notificaciones.EnviarEnlaceActivacionAsync(resultado.Usuario!, urlActivacion);

        model.Exito = true;
        model.NombreUsuario = resultado.Usuario!.NombreCompleto;
        model.EnlaceActivacion = urlActivacion;
        return View(model);
    }

    // ---- HU-4: Activación de cuenta ----

    [HttpGet]
    public async Task<IActionResult> Activar(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return View(new ActivacionViewModel { Estado = ActivacionEstado.TokenInvalido });

        var resultado = await activacion.ActivarPorTokenAsync(token);
        return View(new ActivacionViewModel
        {
            Estado = resultado.Estado,
            NombreUsuario = resultado.NombreUsuario
        });
    }

    // ---- helpers ----

    /// <summary>Normaliza un campo opcional de texto: espacios en blanco → null.</summary>
    private static string? Limpiar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private async Task FirmarSesionAsync(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Role, usuario.Rol),
            new("Documento", usuario.Documento)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });
    }

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
}
