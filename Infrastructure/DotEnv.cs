namespace HuTest.Infrastructure;

/// <summary>
/// Cargador mínimo de archivos <c>.env</c> hacia variables de entorno del proceso.
/// Se ejecuta antes de construir la configuración para que el proveedor de variables
/// de entorno de ASP.NET Core (separador <c>__</c>) las incorpore automáticamente.
/// No sustituye valores ya presentes en el entorno (estos tienen prioridad).
/// </summary>
public static class DotEnv
{
    public static void Load(string? path = null)
    {
        path ??= Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (!File.Exists(path))
            return;

        foreach (var linea in File.ReadAllLines(path))
        {
            var texto = linea.Trim();

            // Ignora líneas vacías y comentarios.
            if (texto.Length == 0 || texto[0] is '#')
                continue;

            var sep = texto.IndexOf('=');
            if (sep <= 0)
                continue;

            var clave = texto[..sep].Trim();
            var valor = texto[(sep + 1)..].Trim().Trim('"', '\'');

            // Respeta variables ya definidas en el entorno real (p. ej. en producción).
            if (Environment.GetEnvironmentVariable(clave) is null)
                Environment.SetEnvironmentVariable(clave, valor);
        }
    }
}
