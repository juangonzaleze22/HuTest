# Especificaciones — Spec-Driven Development (SDD)

Este directorio contiene las **especificaciones** del proyecto **HuTest** (sistema CEPLAN), redactadas siguiendo
un enfoque de **Spec-Driven Development (SDD)**: primero se especifica el *qué* y el *por qué*, luego el *cómo*
técnico, y finalmente las *tareas* ejecutables. **El código de la aplicación se implementa después**, a partir
de estos documentos.

## Origen

Las especificaciones se derivan del diseño Figma *"✅ HU TEST"*
(`fileKey QeuyC4joVTjoRVrKU9OiRh`), que describe el flujo de la Historia de Usuario mediante pantallas, nodos de
decisión, conectores y componentes de estado (toast, diálogo).

## Convenciones

- **Numeración de HU:** carpetas `NNN-slug-descriptivo/` (ej. `001-autenticacion-sesion/`).
- **Sub-historias:** identificadas como `HU-N` dentro de cada `spec.md`.
- **Criterios de aceptación:** en formato **Gherkin** (`Dado / Cuando / Entonces`), identificados como `CA-HU-N.M`.
- **Reglas de negocio:** identificadas como `RN-N`.
- **Tareas:** identificadas como `T-NNN` en `tasks.md`.
- **Trazabilidad:** cada nodo del Figma se mapea a regla → criterio → tarea en `traceability.md`.

## Estructura de una especificación

| Archivo | Propósito |
|---------|-----------|
| `spec.md` | **QUÉ y POR QUÉ.** Contexto, actores, sub-historias, reglas de negocio, estados de UI y criterios de aceptación. Sin detalle técnico. |
| `plan.md` | **CÓMO.** Arquitectura MVC, modelo de datos EF Core, autenticación, mapeo Controllers/Vistas, configuración. |
| `tasks.md` | **Tareas ejecutables** ordenadas y trazadas a criterios de aceptación. |
| `traceability.md` | **Matriz de trazabilidad** Figma → regla → criterio → tarea. |

## Definición de "Listo" (Definition of Done) de una especificación

Una especificación se considera lista cuando:

1. Cada pantalla, nodo de decisión, conector y componente de estado del Figma tiene una regla y un criterio de aceptación.
2. Los parámetros de negocio (umbrales, tiempos) son consistentes entre `spec.md`, `plan.md` y la configuración propuesta.
3. Cada criterio de aceptación es concreto y verificable.
4. Cada tarea de `tasks.md` referencia al menos un criterio de aceptación y nombra archivos reales del proyecto MVC.

## Índice de especificaciones

| ID | Historia de Usuario | Estado |
|----|---------------------|--------|
| [001](001-autenticacion-sesion/spec.md) | Autenticación y Gestión de Sesión | Borrador |

## Stack objetivo

ASP.NET Core **MVC + Razor** (`net10.0`), **EF Core** para persistencia, **autenticación por cookies**.
Convenciones .NET según el skill `dotnet-core-expert` (nullable habilitado, async/await en I/O, DTOs `record`,
inyección de dependencias, consultas EF asíncronas), adaptadas al patrón MVC.
