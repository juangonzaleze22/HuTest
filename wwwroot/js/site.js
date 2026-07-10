// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Sidebar:
//  - Desktop: el botón interno alterna el modo icono (contraído/expandido).
//  - Mobile: se comporta como drawer (modal de izquierda a derecha). El botón del
//    header lo abre; el backdrop o el botón interno lo cierran.
(function () {
    function sidebar() { return document.getElementById('ceplanSidebar'); }
    function backdrop() { return document.getElementById('ceplanSidebarBackdrop'); }
    var esMovil = function () { return window.matchMedia('(max-width: 768px)').matches; };

    // El modo contraído (desktop) se guarda para que sobreviva a la navegación
    // entre páginas (cada vista es una recarga completa de MVC).
    var CLAVE_COLAPSADO = 'ceplan-sidebar-collapsed';
    function guardarColapsado(colapsado) {
        try { localStorage.setItem(CLAVE_COLAPSADO, colapsado ? '1' : '0'); } catch (_) { }
    }

    function abrir() {
        sidebar()?.classList.add('is-open');
        backdrop()?.classList.add('show');
    }
    function cerrar() {
        sidebar()?.classList.remove('is-open');
        backdrop()?.classList.remove('show');
    }

    document.addEventListener('click', function (e) {
        if (e.target.closest('[data-sidebar-open]')) { abrir(); return; }
        if (e.target.closest('[data-sidebar-close]')) { cerrar(); return; }

        if (e.target.closest('[data-sidebar-toggle]')) {
            if (esMovil()) {
                cerrar();
            } else {
                var colapsado = sidebar()?.classList.toggle('ceplan-sidebar--collapsed');
                guardarColapsado(colapsado);
            }
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') cerrar();
    });
})();
