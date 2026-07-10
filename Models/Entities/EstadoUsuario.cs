namespace HuTest.Models.Entities;

/// <summary>Estado de una cuenta de usuario (spec.md §2, RN-5/RN-8).</summary>
public enum EstadoUsuario
{
    /// <summary>Cuenta creada pero aún no activada por el usuario.</summary>
    PendienteActivacion = 0,

    /// <summary>Cuenta activa; puede iniciar sesión.</summary>
    Activo = 1,

    /// <summary>Cuenta bloqueada temporalmente por superar el umbral de CVF.</summary>
    Bloqueado = 2
}
