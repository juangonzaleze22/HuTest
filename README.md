# HuTest — Autenticación y Gestión de Sesión (CEPLAN)

Aplicación web **ASP.NET Core MVC (.NET 10)** que implementa la historia de usuario
**HU-001 — Autenticación y Gestión de Sesión** del sistema CEPLAN.

El objetivo es permitir que un usuario (rol *Operador*) **inicie sesión de forma segura** y que
la plataforma **proteja el acceso** frente a intentos indebidos y sesiones abandonadas.

## ¿Qué hace?

- **Login y validación de credenciales** (documento + contraseña) con autenticación por cookies.
- **Contador de Validaciones Fallidas (CVF)**: cada intento fallido incrementa el contador y, al
  llegar al umbral (5), la cuenta se **bloquea temporalmente** (15 min) y se **notifica por correo**.
- **Desbloqueo automático** al culminar el tiempo de bloqueo (reinicia el CVF).
- **Activación de cuenta** para usuarios nuevos mediante token.
- **Registro** de nuevos usuarios.
- **Cierre de sesión por inactividad** con aviso previo y opción de **extender la sesión**.
- **Perfil de usuario** con detalle de datos personales y laborales.

La especificación funcional completa está en [specs/001-autenticacion-sesion/spec.md](specs/001-autenticacion-sesion/spec.md).

## Stack

- .NET 10 / ASP.NET Core MVC + Razor
- Entity Framework Core + **SQL Server**
- Autenticación por cookies con expiración deslizante (inactividad de sesión)

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (`dotnet --version` debe empezar por `10.`)
- **SQL Server** accesible (LocalDB, Express, o una instancia local/remota).
  La cadena por defecto usa `Server=localhost` con autenticación de Windows.

> La base de datos se **crea y siembra automáticamente** al arrancar (`EnsureCreated` + `DbSeeder`).
> No necesitas ejecutar migraciones manualmente.

## Configuración

Los secretos (cadena de conexión con credenciales, SMTP) se leen de un archivo `.env` en la raíz,
que sobrescribe los valores de `appsettings.json`. **Este paso es opcional**: si no configuras nada,
la app usa `localhost` con autenticación de Windows y el correo se registra en el log en lugar de enviarse.

Para personalizar:

```powershell
# Copia la plantilla y edita los valores
Copy-Item .env.example .env
```

Variables disponibles (ver [.env.example](.env.example)):

| Variable | Descripción |
|----------|-------------|
| `ConnectionStrings__Default` | Cadena de conexión a SQL Server (sobrescribe la de `appsettings.json`) |
| `Smtp__Host`, `Smtp__Port`, `Smtp__From`, `Smtp__Username`, `Smtp__Password` | Configuración del correo saliente. Si `Host` está vacío, el correo se **loguea** en vez de enviarse |

## Cómo levantar el proyecto

Desde la raíz del proyecto (`HuTest/`):

```powershell
# 1. Restaurar dependencias
dotnet restore

# 2. Ejecutar (perfil HTTPS por defecto)
dotnet run
```

La app quedará disponible en:

- **HTTPS:** https://localhost:7253
- **HTTP:** http://localhost:5253

El navegador se abre automáticamente en modo Development. Para forzar solo HTTP:

```powershell
dotnet run --launch-profile http
```

## Usuarios de prueba (sembrados automáticamente)

| Documento | Contraseña | Estado | Uso |
|-----------|------------|--------|-----|
| `12345678` | `Ceplan2025` | Activo | Login normal y perfil (perfil demo del Figma: *Osorio Montes, Adriana*) |
| `87654321` | `Ceplan2025` | Pendiente de activación | Probar activación de cuenta (token: `TOKEN-DEMO-JULY`) |

## Estructura del proyecto

```
HuTest/
├─ Controllers/      # Account (login/registro/activación), Session, Perfil, Home
├─ Services/         # Auth, AccountLock, Activation, Registration, Notification (SMTP)
├─ Models/           # Entities (Usuario, BloqueoCuenta, TokenActivacion), ViewModels, Options
├─ Data/             # AppDbContext + DbSeeder
├─ Infrastructure/   # DotEnv (carga de .env)
├─ Views/            # Razor views
├─ wwwroot/          # Recursos estáticos
└─ specs/            # Especificación funcional, plan y trazabilidad de la HU-001
```

## Parámetros configurables (appsettings.json)

| Parámetro | Valor por defecto | Regla |
|-----------|-------------------|-------|
| `Auth:Cvf:Umbral` | 5 | Intentos fallidos antes de bloquear (RN-3) |
| `Auth:Bloqueo:Minutos` | 15 | Duración del bloqueo temporal (RN-6) |
| `Auth:Sesion:InactividadMinutos` | 20 | Minutos de inactividad antes de cerrar sesión (RN-9) |
| `Auth:Sesion:AvisoSegundos` | 49 | Segundos de aviso previo a la expiración (RN-10) |

> Nota: en el `appsettings.json` actual `InactividadMinutos` está en `1` y `AvisoSegundos` en `49`
> para facilitar las pruebas del flujo de expiración. Ajústalos a `20` para el comportamiento real.
