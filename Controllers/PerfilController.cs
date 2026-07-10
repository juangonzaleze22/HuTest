using System.Security.Claims;
using HuTest.Data;
using HuTest.Models.Entities;
using HuTest.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuTest.Controllers;

[Authorize]
public sealed class PerfilController(AppDbContext db) : Controller
{
    // GET /Perfil  →  detalle del usuario autenticado
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
            return NotFound();

        ViewData["Title"] = "Perfil de usuario";
        ViewData["SidebarColapsado"] = true; // el detalle usa el sidebar en modo icono (ver Figma)

        return View(Mapear(usuario));
    }

    /// <summary>
    /// Construye el ViewModel a partir del <see cref="Usuario"/>. El nombre se guarda como
    /// "Apellidos, Nombres"; los datos de perfil opcionales que no se registraron quedan en
    /// <c>null</c> y la vista los muestra vacíos.
    /// </summary>
    private static PerfilViewModel Mapear(Usuario u)
    {
        var (nombres, primerApellido, segundoApellido) = PartirNombre(u.NombreCompleto);

        return new PerfilViewModel
        {
            NombreMostrado = u.NombreCompleto,
            Iniciales = Iniciales(u.NombreCompleto),
            Estado = u.Estado == EstadoUsuario.Activo ? "Activo" : u.Estado.ToString(),
            EstaActivo = u.Estado == EstadoUsuario.Activo,
            Cargo = u.Cargo,
            Entidad = u.Entidad,
            Nombres = nombres,
            PrimerApellido = primerApellido,
            SegundoApellido = segundoApellido,
            TipoDocumento = u.TipoDocumento,
            NumeroDocumento = u.Documento,
            CorreoPrincipal = u.Email,
            // Datos de perfil opcionales (vacíos si no se informaron en el registro):
            FechaNacimiento = FormatoFecha(u.FechaNacimiento),
            Nacionalidad = u.Nacionalidad,
            Sexo = u.Sexo,
            CorreoSecundario = u.CorreoSecundario,
            TelefonoMovil = u.TelefonoMovil,
            TelefonoSecundario = u.TelefonoSecundario,
            TelefonoSecundarioTipo = u.TelefonoSecundarioTipo,
            TipoContratacion = u.TipoContratacion,
            FechaContratacion = FormatoFecha(u.FechaContratacion),
        };
    }

    /// <summary>Formatea una fecha opcional como "dd / MM / yyyy"; null si no tiene valor.</summary>
    private static string? FormatoFecha(DateOnly? fecha) =>
        fecha?.ToString("dd / MM / yyyy", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Separa "Apellidos, Nombres" en (nombres, primerApellido, segundoApellido).</summary>
    private static (string nombres, string primerApellido, string segundoApellido) PartirNombre(string completo)
    {
        var coma = completo.IndexOf(',');
        if (coma < 0)
            return (completo.Trim(), string.Empty, string.Empty);

        var apellidos = completo[..coma].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var nombres = completo[(coma + 1)..].Trim();
        var primer = apellidos.Length > 0 ? apellidos[0] : string.Empty;
        var segundo = apellidos.Length > 1 ? string.Join(' ', apellidos[1..]) : string.Empty;
        return (nombres, primer, segundo);
    }

    private static string Iniciales(string n)
    {
        if (string.IsNullOrWhiteSpace(n)) return "U";
        var partes = n.Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0) return "U";
        var ini = partes[0][..1];
        if (partes.Length > 1) ini += partes[^1][..1];
        return ini.ToUpperInvariant();
    }
}
