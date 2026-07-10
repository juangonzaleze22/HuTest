using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HuTest.Controllers;

/// <summary>Endpoints AJAX para la gestión de sesión por inactividad (HU-5, HU-6).</summary>
[Authorize]
[Route("[controller]/[action]")]
public sealed class SessionController : Controller
{
    /// <summary>RN-11: renueva la cookie de autenticación, reiniciando el temporizador de inactividad.</summary>
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Extender()
    {
        // Re-emitir el principal actual con propiedades frescas mueve el vencimiento (SlidingExpiration).
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            HttpContext.User,
            new AuthenticationProperties
            {
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow
            });

        return Json(new { extendida = true, hora = DateTimeOffset.UtcNow });
    }

    /// <summary>Permite al cliente confirmar que la sesión sigue activa.</summary>
    [HttpGet]
    public IActionResult Estado() => Json(new { activa = true });
}
