// ============================================================
// Gestión de sesión por inactividad (HU-5 / HU-6, RN-9..RN-12)
// Lee los parámetros desde <body data-inactividad-min data-aviso-seg>.
// ============================================================
(function () {
    "use strict";

    var body = document.body;
    var inactMin = parseInt(body.getAttribute("data-inactividad-min") || "20", 10);
    var avisoSeg = parseInt(body.getAttribute("data-aviso-seg") || "49", 10);
    var totalMs = inactMin * 60 * 1000;
    var avisoMs = avisoSeg * 1000;

    console.log("Gestión de sesión: inactividad=" + inactMin + " min, aviso=" + avisoSeg + " seg");

    var dialogo = document.getElementById("dialogoExpiracion");
    var cuenta = document.getElementById("dlgExpCuenta");
    var btnExtender = document.getElementById("btnExtenderSesion");
    var btnCerrar = document.getElementById("btnCerrarSesionAhora");
    var formLogout = document.getElementById("formLogoutExpirado");

    // Sólo aplica en páginas autenticadas (el diálogo sólo existe en _Layout).
    if (!dialogo || !formLogout) return;

    var avisoTimer = null;
    var expiraTimer = null;
    var cuentaInterval = null;

    function limpiar() {
        clearTimeout(avisoTimer);
        clearTimeout(expiraTimer);
        clearInterval(cuentaInterval);
    }

    function expirar() {
        limpiar();
        formLogout.submit(); // -> /Account/Login?expirado=true (dispara el toast)
    }

    function mostrarAviso() {
        dialogo.classList.add("show");
        var restante = avisoSeg;
        cuenta.textContent = restante;
        cuentaInterval = setInterval(function () {
            restante -= 1;
            cuenta.textContent = restante > 0 ? restante : 0;
            if (restante <= 0) clearInterval(cuentaInterval);
        }, 1000);
        expiraTimer = setTimeout(expirar, avisoMs);
    }

    function reiniciar() {
        limpiar();
        dialogo.classList.remove("show");
        avisoTimer = setTimeout(mostrarAviso, Math.max(totalMs - avisoMs, 0));
    }

    // RN-11: extender la sesión renueva la cookie y reinicia el temporizador.
    btnExtender.addEventListener("click", function () {
        fetch("/Session/Extender", {
            method: "POST",
            headers: { "X-Requested-With": "XMLHttpRequest" }
        }).then(reiniciar).catch(reiniciar);
    });

    if (btnCerrar) btnCerrar.addEventListener("click", expirar);

    // La actividad del usuario reinicia el conteo (salvo mientras el aviso está visible).
    ["mousemove", "keydown", "click", "scroll", "touchstart"].forEach(function (ev) {
        document.addEventListener(ev, function () {
            if (!dialogo.classList.contains("show")) reiniciar();
        }, { passive: true });
    });

    reiniciar();
})();
