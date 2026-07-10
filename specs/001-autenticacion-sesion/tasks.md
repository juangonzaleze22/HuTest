# HU-001 — Tareas de implementación

> Tareas ejecutables ordenadas por dependencia. Cada tarea indica archivos afectados, criterios de aceptación
> cubiertos y dependencias. Los identificadores `CA-HU-*` y `RN-*` remiten a [`spec.md`](spec.md);
> el detalle técnico está en [`plan.md`](plan.md).

## Leyenda
- **Estado:** ☐ pendiente · ☑ hecho
- **Dep.:** tareas de las que depende.

---

## Fase 0 — Infraestructura

### ☐ T-001 — Dependencias NuGet
Agregar paquetes: EF Core (SqlServer/Sqlite), `EFCore.Design`, `AspNetCore.Identity` (para `PasswordHasher`),
paquetes de test (`xUnit`, `Mvc.Testing`).
- **Archivos:** `HuTest.csproj`
- **CA:** — (habilitador) · **Dep.:** —

### ☐ T-002 — Configuración tipada
Crear `AuthOptions` y sección `Auth` en `appsettings.json` (CVF=5, Bloqueo=15min, Inactividad=20min, Aviso=49s);
enlazar con `IOptions<AuthOptions>` en `Program.cs`.
- **Archivos:** `appsettings.json`, `Models/Options/AuthOptions.cs`, `Program.cs`
- **CA:** RN-3/6/9/10 · **Dep.:** T-001

---

## Fase 1 — Datos

### ☐ T-003 — Entidades EF Core
Crear `Usuario`, `BloqueoCuenta`, `TokenActivacion` con sus propiedades (ver `plan.md` §2).
- **Archivos:** `Models/Entities/*.cs`
- **CA:** RN-1..RN-8 · **Dep.:** T-001

### ☐ T-004 — DbContext + configuración
Crear `AppDbContext` con los `DbSet` y configuraciones; registrar en DI con la cadena de conexión.
- **Archivos:** `Data/AppDbContext.cs`, `Program.cs`
- **CA:** — · **Dep.:** T-003

### ☐ T-005 — Migración inicial + seed
Generar migración `InitialCreate` y datos semilla (usuario de prueba activo, usuario pendiente de activación).
- **Archivos:** `Migrations/*`, `Data/DbSeeder.cs`
- **CA:** — · **Dep.:** T-004

---

## Fase 2 — Servicios (lógica de negocio)

### ☐ T-006 — AccountLockService (CVF + bloqueo + desbloqueo)
Implementar `RegistrarIntentoFallidoAsync`, `EstaBloqueadaAsync` (desbloqueo lazy y reinicio de CVF),
`RegistrarLoginExitosoAsync`. Umbral y duración desde configuración.
- **Archivos:** `Services/IAccountLockService.cs`, `Services/AccountLockService.cs`
- **CA:** CA-HU-2.1, CA-HU-2.2, CA-HU-3.1 · RN-2/3/5/6/7 · **Dep.:** T-004, T-002

### ☐ T-007 — INotificationService (correo N2)
Interfaz + `EmailNotificationService` (SMTP configurable; mock/log en desarrollo). Invocado al bloquear.
- **Archivos:** `Services/INotificationService.cs`, `Services/EmailNotificationService.cs`
- **CA:** RN-4 · **Dep.:** T-006

### ☐ T-008 — AuthService (validación de credenciales)
Validar credenciales con `PasswordHasher`, verificar estado (activo/bloqueado), emitir `ClaimsPrincipal`.
- **Archivos:** `Services/IAuthService.cs`, `Services/AuthService.cs`
- **CA:** CA-HU-1.1, CA-HU-1.2, CA-HU-4.2 · RN-1/8 · **Dep.:** T-006

### ☐ T-009 — ActivationService
Validar token, activar cuenta, marcar token usado/expirado.
- **Archivos:** `Services/IActivationService.cs`, `Services/ActivationService.cs`
- **CA:** CA-HU-4.1 · RN-8 · **Dep.:** T-004

---

## Fase 3 — Autenticación y controllers

### ☐ T-010 — Auth por cookies en Program.cs
Configurar `AddAuthentication().AddCookie(...)` con `ExpireTimeSpan=20min`, `SlidingExpiration`, `LoginPath`;
`app.UseAuthentication()`.
- **Archivos:** `Program.cs`
- **CA:** RN-9 · **Dep.:** T-002

### ☐ T-011 — AccountController (Login/Logout)
GET/POST `Login`; en POST orquesta `AuthService` + `AccountLockService`; `Logout`. Redirige a `Bloqueada` si aplica.
- **Archivos:** `Controllers/AccountController.cs`, `Models/ViewModels/LoginViewModel.cs`
- **CA:** CA-HU-1.1, CA-HU-1.2, CA-HU-2.1, CA-HU-2.2 · **Dep.:** T-008, T-006, T-010

### ☐ T-012 — AccountController (Activar / Bloqueada)
Acciones `Activar` (GET/POST con token) y `Bloqueada` (muestra tiempo restante).
- **Archivos:** `Controllers/AccountController.cs`, `Models/ViewModels/ActivacionViewModel.cs`
- **CA:** CA-HU-4.1, CA-HU-4.2, CA-HU-2.2 · **Dep.:** T-009, T-006

### ☐ T-013 — SessionController (Extender/Estado)
Endpoints AJAX `Extender` (renueva cookie) y `Estado`.
- **Archivos:** `Controllers/SessionController.cs`
- **CA:** CA-HU-6.1 · RN-11 · **Dep.:** T-010

---

## Fase 4 — Vistas Razor y cliente

### ☐ T-014 — Parciales de UI base
`_Header`, `_Sidebar`, `_Footer` según Figma, integrados en `_Layout.cshtml` (Bootstrap existente).
- **Archivos:** `Views/Shared/_Header.cshtml`, `_Sidebar.cshtml`, `_Footer.cshtml`, `_Layout.cshtml`
- **CA:** Estados UI §5 · **Dep.:** T-010

### ☐ T-015 — Vistas Login / Bloqueada / Activar
Formularios y mensajes de error/estado.
- **Archivos:** `Views/Account/{Login,Bloqueada,Activar}.cshtml`
- **CA:** CA-HU-1.*, CA-HU-2.2, CA-HU-4.* · **Dep.:** T-011, T-012

### ☐ T-016 — Diálogo de aviso y toast de expiración
Parciales `_DialogoExpiracion` (modal + cuenta regresiva) y `_ToastSesion`.
- **Archivos:** `Views/Shared/_DialogoExpiracion.cshtml`, `_ToastSesion.cshtml`
- **CA:** CA-HU-5.1, CA-HU-6.2 · **Dep.:** T-014

### ☐ T-017 — JS de timeout de sesión
`session-timeout.js`: detección de inactividad, aviso a ~19:11, extender, expiración → toast + redirect.
Tiempos inyectados desde configuración.
- **Archivos:** `wwwroot/js/session-timeout.js`, `_Layout.cshtml`
- **CA:** CA-HU-5.1, CA-HU-6.1, CA-HU-6.2 · RN-10/11/12 · **Dep.:** T-013, T-016

---

## Fase 5 — Pruebas y verificación

### ☐ T-018 — Pruebas unitarias de servicios
`AccountLockService` (CVF, bloqueo al 5º, desbloqueo lazy, reinicio), `ActivationService`.
- **Archivos:** `HuTest.Tests/Services/*`
- **CA:** CA-HU-2.1, CA-HU-3.1, CA-HU-4.1 · **Dep.:** T-006, T-009

### ☐ T-019 — Pruebas de integración de flujos
`WebApplicationFactory<Program>`: login ok/inválido, cuenta bloqueada, activación.
- **Archivos:** `HuTest.Tests/Integration/*`
- **CA:** CA-HU-1.*, CA-HU-2.2, CA-HU-4.2 · **Dep.:** T-011, T-012

### ☐ T-020 — Verificación end-to-end + trazabilidad
Ejecutar `dotnet build` y `dotnet test`; recorrer manualmente cada estado de UI; actualizar `traceability.md`.
- **Archivos:** `specs/001-autenticacion-sesion/traceability.md`
- **CA:** todos · **Dep.:** T-014..T-019

---

## Orden sugerido
`T-001 → T-002 → T-003 → T-004 → T-005 → T-006 → T-007 → T-008 → T-009 → T-010 → T-011 → T-012 → T-013 → T-014 → T-015 → T-016 → T-017 → T-018 → T-019 → T-020`
