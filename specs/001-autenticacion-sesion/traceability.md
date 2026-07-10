# HU-001 — Matriz de trazabilidad

Relaciona cada elemento del Figma *"✅ HU TEST"* (`fileKey QeuyC4joVTjoRVrKU9OiRh`) con su regla de negocio
(`RN-*`), criterio de aceptación (`CA-HU-*`) y tarea de implementación (`T-*`). Sirve como checklist de
**completitud** de la especificación.

## 1. Elementos del Figma → Regla → Criterio → Tarea

| Elemento Figma (tipo · nombre · nodo) | Regla | Criterio | Tarea |
|---------------------------------------|-------|----------|-------|
| Conector · "Ingresa credenciales" `7291:75571` | RN-1 | CA-HU-1.1 | T-011, T-015 |
| Conector · "Clic en ingresar (Valida Credenciales)" `7291:75576` | RN-1 | CA-HU-1.1/1.2 | T-008, T-011 |
| Decisión · "¿Las credenciales son válidas?" `7291:77386` | RN-1, RN-2, RN-7 | CA-HU-1.1/1.2 | T-008, T-011 |
| Conector · "Sí" `7291:79792` | RN-7 | CA-HU-1.1 | T-011 |
| Conector · "No" `7291:77377` / `7291:77390` | RN-2 | CA-HU-1.2 | T-006, T-011 |
| Tarjeta error · "=5" `7291:78030` | RN-3 | CA-HU-2.1 | T-002, T-006 |
| Tarjeta · "Notificar por correo… (N2)" `7291:79743` | RN-4 | CA-HU-2.1 | T-007 |
| Decisión · "¿La cuenta está bloqueada?" (CVF) `7291:77382` | RN-5 | CA-HU-2.2 | T-006, T-012 |
| Decisión · "ID de la cuenta está bloqueada?" `7291:75608` | RN-5 | CA-HU-2.2 | T-006, T-011 |
| Conector · "Culminó el tiempo de bloqueo" `7291:125997` | RN-6 | CA-HU-3.1 | T-006 |
| Pantalla · "Activación de cuenta" `7288:67426` | RN-8 | CA-HU-4.1/4.2 | T-009, T-012, T-015 |
| Decisión · "¿El usuario está inactivo por 20 minutos?" `7291:79803` | RN-9 | CA-HU-5.1 | T-010, T-017 |
| Conector · "La sesión expiró" `7291:84299` | RN-12 | CA-HU-6.2 | T-016, T-017 |
| Diálogo · "…expirar en 49 segundos… Extender sesión" `7291:79850`/`4106:48236` | RN-10 | CA-HU-5.1 | T-016, T-017 |
| Conector · "Extendió la sesión" `7291:84303` | RN-11 | CA-HU-6.1 | T-013, T-017 |
| Toast · "Su sesión ha expirado debido a inactividad…" `7291:82476`/`7184:111694` | RN-12 | CA-HU-6.2 | T-016, T-017 |
| Componente · Header (logo, búsqueda, ayuda, notif., perfil "Operador") `5487:10005` | Estados UI §5 | — | T-014 |
| Componente · Sidebar "PEI Operador" `5487:9938` | Estados UI §5 | — | T-014 |
| Componente · Footer `cp-pie-pagina` `5487:9166` | Estados UI §5 | — | T-014 |

## 2. Cobertura por criterio de aceptación

| Criterio | Reglas | Tareas | Cubierto |
|----------|--------|--------|----------|
| CA-HU-1.1 Login exitoso | RN-1, RN-7 | T-008, T-011, T-015 | ✅ |
| CA-HU-1.2 Credenciales inválidas | RN-1, RN-2 | T-008, T-011, T-015 | ✅ |
| CA-HU-2.1 Bloqueo al 5º intento | RN-3, RN-4 | T-006, T-007 | ✅ |
| CA-HU-2.2 Intento durante bloqueo | RN-5 | T-006, T-012 | ✅ |
| CA-HU-3.1 Desbloqueo automático | RN-6, RN-7 | T-006 | ✅ |
| CA-HU-4.1 Activación exitosa | RN-8 | T-009, T-012, T-015 | ✅ |
| CA-HU-4.2 Login sin activar | RN-8 | T-008, T-011 | ✅ |
| CA-HU-5.1 Aviso antes de expirar | RN-9, RN-10 | T-016, T-017 | ✅ |
| CA-HU-6.1 Extender sesión | RN-11 | T-013, T-017 | ✅ |
| CA-HU-6.2 Sesión expirada | RN-12 | T-016, T-017 | ✅ |

## 3. Consistencia de parámetros

| Parámetro | spec.md | plan.md | appsettings (propuesto) |
|-----------|---------|---------|--------------------------|
| Umbral CVF | 5 | `CVF.Umbral=5` | `Auth:Cvf:Umbral=5` | 
| Inactividad | 20 min | `ExpireTimeSpan=20min` | `Auth:Sesion:InactividadMinutos=20` |
| Aviso previo | 49 s | ~19:11 (20min−49s) | `Auth:Sesion:AvisoSegundos=49` |
| Duración bloqueo | 15 min *(propuesto)* | `Bloqueo.Minutos` | `Auth:Bloqueo:Minutos=15` |

> ⚠️ **Pendiente:** confirmar la **duración del bloqueo** (15 min es propuesta, no consta en Figma).

## 4. Elementos del Figma no incluidos en esta HU (fuera de alcance)

- `progress bar` / `Stepper` de `page header` — pertenecen a otros flujos de "Gestión de usuarios", no al núcleo de auth/sesión.
- Búsqueda del header y notificaciones (íconos) — se muestran como UI base pero su lógica no forma parte de esta HU.
