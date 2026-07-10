# HU-001 — Plan técnico (CÓMO)

> **Tipo:** Diseño técnico de implementación
> **Stack:** ASP.NET Core **MVC + Razor** (`net10.0`) · **EF Core** · **Autenticación por cookies**
> **Base:** proyecto `HuTest` existente (se extiende, no se reescribe)
> **Convenciones:** skill `dotnet-core-expert` — nullable habilitado, `async/await` en I/O, DTOs/ViewModels
> `record` donde aplique, inyección de dependencias, consultas EF asíncronas (`AsNoTracking`, `ToListAsync`).

---

## 1. Arquitectura de proyecto

Se mantiene el patrón MVC del proyecto actual y se añade una capa de **servicios** para no mezclar lógica de
negocio en los controllers.

```
HuTest/
├─ Controllers/
│  ├─ AccountController.cs      # Login, Logout, Activar, Bloqueada
│  └─ SessionController.cs      # Heartbeat/Ping, Extender, Estado (AJAX)
├─ Models/
│  ├─ Entities/                 # Entidades EF Core (persistencia)
│  │  ├─ Usuario.cs
│  │  ├─ BloqueoCuenta.cs
│  │  └─ TokenActivacion.cs
│  └─ ViewModels/               # Modelos de vista Razor
│     ├─ LoginViewModel.cs
│     └─ ActivacionViewModel.cs
├─ Services/
│  ├─ IAuthService.cs / AuthService.cs
│  ├─ IAccountLockService.cs / AccountLockService.cs
│  ├─ INotificationService.cs / EmailNotificationService.cs
│  └─ IActivationService.cs / ActivationService.cs
├─ Data/
│  └─ AppDbContext.cs
├─ Views/
│  ├─ Account/{Login,Activar,Bloqueada}.cshtml
│  └─ Shared/{_Header,_Sidebar,_Footer,_ToastSesion,_DialogoExpiracion}.cshtml
├─ wwwroot/js/session-timeout.js
└─ Program.cs                   # DI + auth por cookies + DbContext
```

## 2. Modelo de datos (EF Core)

| Entidad | Campos clave | Notas |
|---------|--------------|-------|
| **Usuario** | `Id`, `Email`, `PasswordHash`, `Estado` (`PendienteActivacion` \| `Activo` \| `Bloqueado`), `Cvf` (int), `NombreCompleto`, `Rol` | `Cvf` = Contador de Validaciones Fallidas (RN-2). `PasswordHash` con hashing seguro (ver §4). |
| **BloqueoCuenta** | `Id`, `UsuarioId`, `FechaInicio`, `FechaFin`, `Activo` | Bloqueo temporal (RN-3/RN-5/RN-6). El desbloqueo se evalúa comparando `FechaFin` con la hora actual. |
| **TokenActivacion** | `Id`, `UsuarioId`, `Token`, `FechaExpira`, `Usado` | Activación de cuenta (RN-8 / HU-4). |

`AppDbContext` expone `DbSet<Usuario>`, `DbSet<BloqueoCuenta>`, `DbSet<TokenActivacion>`; configuración vía
`ApplyConfigurationsFromAssembly`. Migración inicial `InitialCreate`. Proveedor por defecto: **SQL Server**
(cadena en `appsettings.json`); alternativa **SQLite** para desarrollo local.

> **Nota sobre inactividad (RN-9):** la inactividad de sesión se gestiona con la **cookie de autenticación**
> (`SlidingExpiration`) + control en cliente (§6), no requiere entidad persistente. No se crea tabla `Sesion`.

## 3. Estrategia de bloqueo/desbloqueo (RN-3, RN-5, RN-6, RN-7)

- **AccountLockService** encapsula: `RegistrarIntentoFallidoAsync(email)`, `EstaBloqueadaAsync(email)`,
  `ReiniciarCvfAsync(email)`, `RegistrarLoginExitosoAsync(email)`.
- **Desbloqueo perezoso (lazy):** al evaluar `EstaBloqueadaAsync`, si `FechaFin <= now` se marca el bloqueo
  como inactivo y se reinicia `Cvf = 0`. Evita depender de un job en background para el MVP.
- **Umbral y duración:** desde configuración (`CVF.Umbral`, `Bloqueo.Minutos`).

## 4. Autenticación

- **Esquema:** `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(...)`.
- **Opciones de cookie:** `LoginPath = /Account/Login`, `ExpireTimeSpan = 20 min`, `SlidingExpiration = true`,
  `Cookie.HttpOnly = true`, `Cookie.SecurePolicy = Always`.
- **Password hashing:** `PasswordHasher<Usuario>` (de `Microsoft.AspNetCore.Identity`) o `ASP.NET Core Identity`
  completo. **Decisión por defecto:** cookies + tabla `Usuario` propia con `PasswordHasher<Usuario>` (más ligero
  que Identity completo). *(Reevaluar si se necesita gestión de usuarios/roles avanzada.)*
- Al autenticar se emite un `ClaimsPrincipal` con `Name`, `Role` ("Operador") para el header/sidebar.

## 5. Mapeo de UI (Controllers → Actions → Vistas)

| Sub-historia | Controller.Action | Método | Vista | Criterios |
|--------------|-------------------|--------|-------|-----------|
| HU-1 | `Account.Login` (GET/POST) | GET muestra form; POST valida | `Account/Login.cshtml` | CA-HU-1.1/1.2 |
| HU-2 | `Account.Login` (POST) + `AccountLockService` | POST | `Account/Login.cshtml` (error) | CA-HU-2.1 |
| HU-2/HU-5 | `Account.Bloqueada` | GET | `Account/Bloqueada.cshtml` | CA-HU-2.2 |
| HU-3 | `AccountLockService` (lazy en `Login`) | — | — | CA-HU-3.1 |
| HU-4 | `Account.Activar` (GET/POST) | GET valida token; POST activa | `Account/Activar.cshtml` | CA-HU-4.1/4.2 |
| HU-5/HU-6 | `Session.Estado`, `Session.Extender` | POST (AJAX) | parciales `_DialogoExpiracion`, `_ToastSesion` | CA-HU-5.1, CA-HU-6.1/6.2 |

**Parciales Razor reutilizables** (reutilizan Bootstrap ya presente en `wwwroot/lib/bootstrap`):
`_Header` (logo, búsqueda, ayuda, notificaciones, perfil "Operador"), `_Sidebar` ("PEI Operador"),
`_Footer` (`cp-pie-pagina`), `_DialogoExpiracion` (modal), `_ToastSesion` (toast). Se integran en `_Layout.cshtml`.

## 6. Timeout de sesión en cliente (HU-5, HU-6)

`wwwroot/js/session-timeout.js`:
- Escucha eventos de actividad (`mousemove`, `keydown`, `click`, `scroll`) para resetear el temporizador local.
- A los **~19:11** (20 min − 49 s) muestra `_DialogoExpiracion` con cuenta regresiva de 49 s.
- Botón **"Extender sesión"** → `POST /Session/Extender` (renueva la cookie); reinicia el temporizador.
- Si el contador llega a 0 sin respuesta → `POST /Session/Logout` (o redirect a `Login` con flag) y se muestra
  el toast de sesión expirada.
- Los tiempos se inyectan desde configuración a la vista (data-attributes) para mantener un único origen de verdad.

## 7. Notificación por correo N2 (RN-4)

- `INotificationService.EnviarCuentaBloqueadaAsync(usuario)`.
- Implementación `EmailNotificationService` con SMTP configurable; en desarrollo, implementación *mock*/log.
- Se invoca desde `AccountLockService` al momento del bloqueo. Envío `async`, sin bloquear la respuesta del login.

## 8. Configuración (`appsettings.json`)

```json
{
  "Auth": {
    "Cvf": { "Umbral": 5 },
    "Bloqueo": { "Minutos": 15 },
    "Sesion": { "InactividadMinutos": 20, "AvisoSegundos": 49 }
  },
  "ConnectionStrings": { "Default": "..." },
  "Smtp": { "Host": "", "Port": 587, "From": "no-reply@ceplan.gob.pe" }
}
```
Enlazado con `IOptions<AuthOptions>` para tipado fuerte. **Los valores deben coincidir con `spec.md` §3.**

## 9. Pruebas

- **Unitarias (xUnit):** `AccountLockService` (incremento CVF, bloqueo al 5º, desbloqueo lazy, reinicio en login
  exitoso), `ActivationService`.
- **Integración (`WebApplicationFactory<Program>`):** flujo de login (200/redirect), login inválido, cuenta
  bloqueada, activación. BD en memoria/SQLite para tests.
- Cada prueba se nombra según el criterio de aceptación que cubre (ej. `Login_CredencialesInvalidas_IncrementaCvf`).

## 10. Dependencias NuGet previstas

- `Microsoft.EntityFrameworkCore.SqlServer` (o `.Sqlite`)
- `Microsoft.EntityFrameworkCore.Design` (migraciones)
- `Microsoft.AspNetCore.Identity` (solo `PasswordHasher<T>`)
- `Microsoft.AspNetCore.Authentication.Cookies` (incluido en el framework)
- Test: `xUnit`, `Microsoft.AspNetCore.Mvc.Testing`
