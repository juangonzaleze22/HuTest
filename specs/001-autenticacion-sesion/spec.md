# HU-001 — Autenticación y Gestión de Sesión

> **Tipo:** Especificación funcional (QUÉ y POR QUÉ)
> **Estado:** Borrador
> **Origen:** Figma *"✅ HU TEST"* (`fileKey QeuyC4joVTjoRVrKU9OiRh`, canvas `7174:99647`)
> **Sistema:** CEPLAN — proyecto `HuTest`

---

## 1. Resumen y objetivo

El sistema debe permitir que un usuario **inicie sesión de forma segura** validando sus credenciales, y que
la plataforma **proteja el acceso** frente a intentos indebidos y sesiones abandonadas. Esto abarca:

- Validación de credenciales en el login.
- Contador de intentos fallidos (**CVF — Contador de Validaciones Fallidas**) con **bloqueo temporal** de la
  cuenta y **notificación por correo** al superar el umbral.
- **Desbloqueo automático** al culminar el tiempo de bloqueo.
- **Activación de cuenta** para usuarios nuevos.
- **Cierre de sesión por inactividad** (20 minutos) con **aviso previo** y opción de **extender la sesión**.

**Valor de negocio:** garantizar la seguridad del acceso al sistema CEPLAN, mitigando ataques de fuerza bruta
y exposición de sesiones desatendidas, con una experiencia clara para el usuario (operador).

## 2. Actores

| Actor | Descripción |
|-------|-------------|
| **Operador** | Usuario final del sistema (rol mostrado en el header como "Operador"). Inicia sesión, opera y puede quedar bloqueado o expirar su sesión. |
| **Sistema** | Aplicación web CEPLAN. Valida credenciales, cuenta intentos, bloquea/desbloquea, gestiona la sesión. |
| **Servicio de correo** | Envía la notificación de cuenta bloqueada (referida en el Figma como **N2**). |

## 3. Reglas de negocio

| ID | Regla | Valor | Origen en Figma |
|----|-------|-------|-----------------|
| **RN-1** | El login valida usuario + contraseña contra el sistema. | — | Conector "Clic en ingresar (Valida Credenciales)" |
| **RN-2** | Cada intento con credenciales inválidas incrementa el CVF de la cuenta. | +1 por fallo | Decisión "¿Las credenciales son válidas?" → No |
| **RN-3** | Al alcanzar el umbral de CVF, la cuenta se **bloquea temporalmente**. | **CVF = 5** | Tarjeta hu-error "=5" |
| **RN-4** | Al bloquear la cuenta se envía un **correo de notificación de cuenta bloqueada (N2)**. | — | Tarjeta "Notificar por correo al usuario informando bloqueo temporal (N2…)" |
| **RN-5** | Mientras la cuenta esté bloqueada, no se permite iniciar sesión aunque las credenciales sean correctas. | — | Decisión "¿La cuenta está bloqueada?" |
| **RN-6** | El bloqueo se levanta **automáticamente** al culminar el tiempo de bloqueo, y el CVF se reinicia. | Duración = **15 min** *(propuesto, ver §7)* | Conector "Culminó el tiempo de bloqueo" |
| **RN-7** | Un login exitoso **reinicia el CVF a 0**. | CVF = 0 | Flujo feliz "Sí" |
| **RN-8** | La cuenta debe estar **activada** para poder iniciar sesión. | — | Pantalla "Activación de cuenta" |
| **RN-9** | La sesión se cierra tras **20 minutos de inactividad** del usuario. | **20 min** | Decisión "¿El usuario está inactivo por 20 minutos?" |
| **RN-10** | Antes de expirar, se muestra un **aviso previo** con cuenta regresiva y opción de extender. | Aviso ≈ **49 s** antes | Diálogo "Su sesión está a punto de expirar en 49 segundos… Extender sesión" |
| **RN-11** | Si el usuario **extiende** la sesión, el temporizador de inactividad se reinicia. | — | Conector "Extendió la sesión" |
| **RN-12** | Si el usuario **no responde**, la sesión expira, se cierra y se muestra un aviso para volver a iniciar sesión. | — | Conector "La sesión expiró" + toast |

> **Parámetros configurables:** `CVF.Umbral = 5`, `Bloqueo.Minutos = 15` *(propuesto)*,
> `Sesion.InactividadMinutos = 20`, `Sesion.AvisoSegundos = 49`.

## 4. Sub-historias

### HU-1 — Login y validación de credenciales
Como **operador**, quiero **ingresar mis credenciales** para **acceder al sistema**.

### HU-2 — Contador de intentos fallidos (CVF) y bloqueo temporal
Como **sistema**, debo **contar los intentos fallidos** y **bloquear temporalmente** la cuenta al llegar a 5,
**notificando por correo** al usuario, para **proteger la cuenta** frente a accesos indebidos.

### HU-3 — Desbloqueo automático
Como **operador con cuenta bloqueada**, quiero que **al culminar el tiempo de bloqueo** mi cuenta se
**desbloquee automáticamente**, para poder **volver a intentar** sin intervención manual.

### HU-4 — Activación de cuenta
Como **usuario nuevo**, quiero **activar mi cuenta** para **habilitar mi acceso** al sistema.

### HU-5 — Timeout de sesión por inactividad con aviso previo
Como **operador**, quiero **ser advertido antes de que mi sesión expire** por inactividad, para **no perder mi
trabajo** de forma inesperada.

### HU-6 — Extender sesión / expiración
Como **operador**, quiero poder **extender mi sesión** desde el aviso, y si no lo hago, **ser informado** de que
la sesión expiró para **volver a iniciar sesión**.

## 5. Estados de UI (derivados del Figma)

| Estado | Descripción | Componente Figma |
|--------|-------------|------------------|
| **Normal / autenticado** | Header (logo, búsqueda, ayuda, notificaciones, perfil "Operador"), sidebar "PEI Operador", contenido, footer. | `Header 5487:10005`, `Sidebar 5487:9938`, footer `5487:9166` |
| **Login** | Formulario de credenciales. | Pantalla "Gestión de usuarios" `7295:55623` |
| **Error de credenciales** | Mensaje de credenciales inválidas; CVF incrementado. | Decisión "¿Las credenciales son válidas?" → No |
| **Cuenta bloqueada** | Aviso de bloqueo temporal; login deshabilitado hasta desbloqueo. | Decisión "¿La cuenta está bloqueada?" |
| **Activación de cuenta** | Pantalla de activación. | Pantalla "Activación de cuenta" `7288:67426` |
| **Aviso de expiración** | Diálogo modal con cuenta regresiva y botón "Extender sesión". | `dialog 4106:48236`, pantalla `7291:79850` |
| **Sesión expirada** | Toast informando expiración por inactividad e invitando a reingresar. | `toast alert 7184:111694`, pantalla `7291:82476` |

## 6. Criterios de aceptación (Gherkin)

### CA-HU-1 — Login
```gherkin
Escenario: CA-HU-1.1 Login exitoso
  Dado un operador con cuenta activa y no bloqueada
  Cuando ingresa credenciales válidas y presiona "Ingresar"
  Entonces el sistema inicia su sesión
  Y reinicia su contador CVF a 0
  Y lo redirige a la pantalla principal

Escenario: CA-HU-1.2 Credenciales inválidas
  Dado un operador con cuenta activa y no bloqueada
  Cuando ingresa credenciales inválidas y presiona "Ingresar"
  Entonces el sistema no inicia sesión
  Y muestra un mensaje de credenciales inválidas
  Y incrementa el contador CVF en 1
```

### CA-HU-2 — CVF y bloqueo temporal
```gherkin
Escenario: CA-HU-2.1 Bloqueo al alcanzar el umbral
  Dado un operador con CVF en 4
  Cuando ingresa credenciales inválidas por quinta vez
  Entonces el CVF llega a 5
  Y la cuenta queda bloqueada temporalmente
  Y se envía el correo de notificación de cuenta bloqueada (N2)

Escenario: CA-HU-2.2 Intento durante el bloqueo
  Dado un operador con la cuenta bloqueada temporalmente
  Cuando ingresa credenciales (válidas o inválidas)
  Entonces el sistema no inicia sesión
  Y muestra que la cuenta está bloqueada e indica el tiempo restante
```

### CA-HU-3 — Desbloqueo automático
```gherkin
Escenario: CA-HU-3.1 Desbloqueo al culminar el tiempo
  Dado un operador con la cuenta bloqueada temporalmente
  Cuando culmina el tiempo de bloqueo configurado
  Entonces la cuenta queda desbloqueada automáticamente
  Y el contador CVF se reinicia a 0
  Y el operador puede volver a intentar iniciar sesión
```

### CA-HU-4 — Activación de cuenta
```gherkin
Escenario: CA-HU-4.1 Activación exitosa
  Dado un usuario nuevo con una cuenta pendiente de activación
  Cuando completa el proceso de activación con un token válido
  Entonces su cuenta queda activada
  Y puede iniciar sesión

Escenario: CA-HU-4.2 Intento de login sin activar
  Dado un usuario con cuenta pendiente de activación
  Cuando intenta iniciar sesión con credenciales válidas
  Entonces el sistema no inicia sesión
  Y le indica que debe activar su cuenta
```

### CA-HU-5 — Aviso de expiración por inactividad
```gherkin
Escenario: CA-HU-5.1 Aviso antes de expirar
  Dado un operador con sesión iniciada
  Cuando permanece inactivo hasta ~49 segundos antes de cumplir 20 minutos
  Entonces se muestra un diálogo de aviso de expiración con cuenta regresiva
  Y ofrece la opción "Extender sesión"
```

### CA-HU-6 — Extender sesión / expiración
```gherkin
Escenario: CA-HU-6.1 Extender la sesión
  Dado que se muestra el diálogo de aviso de expiración
  Cuando el operador selecciona "Extender sesión"
  Entonces el temporizador de inactividad se reinicia
  Y la sesión continúa activa sin recargar el trabajo

Escenario: CA-HU-6.2 Sesión expirada por inactividad
  Dado que se muestra el diálogo de aviso de expiración
  Cuando el operador no responde y culminan los 20 minutos de inactividad
  Entonces la sesión se cierra
  Y se muestra un toast: "Su sesión ha expirado debido a inactividad. Por favor, inicie sesión nuevamente."
  Y se redirige al login
```

## 7. Fuera de alcance y supuestos

**Fuera de alcance (de esta HU):**
- Recuperación/cambio de contraseña.
- Gestión de roles y permisos más allá del rol "Operador" mostrado.
- Registro de auditoría avanzado (más allá de contar intentos).

**Supuestos y decisiones pendientes de confirmar:**
- **Duración del bloqueo temporal (RN-6):** se propone **15 minutos** por defecto (el Figma no lo especifica).
- **Mecanismo de autenticación:** cookies vs ASP.NET Core Identity — se decide en `plan.md`; por defecto **cookies + tabla de usuarios propia**.
- **Origen de usuarios/credenciales:** se asume **BD local** (no integración con directorio externo) salvo indicación contraria.
- El contenido textual exacto de la pantalla de activación se refinará con capturas del Figma durante la implementación.
