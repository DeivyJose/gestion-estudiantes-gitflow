const usuarioAutenticado =
    document.getElementById("usuario-autenticado");

const botonCerrarSesion =
    document.getElementById("btn-cerrar-sesion");

async function verificarSesion() {
    try {
        const respuesta = await fetch("/api/auth/status", {
            credentials: "same-origin"
        });

        if (!respuesta.ok) {
            window.location.replace("/");
            return;
        }

        const datos = await respuesta.json();

        usuarioAutenticado.textContent =
            `Usuario: ${datos.usuario}`;
    } catch {
        window.location.replace("/");
    }
}

botonCerrarSesion.addEventListener("click", async () => {
    botonCerrarSesion.disabled = true;
    botonCerrarSesion.textContent = "Cerrando...";

    try {
        await fetch("/api/auth/logout", {
            method: "POST",
            credentials: "same-origin"
        });
    } finally {
        window.location.replace("/");
    }
});

verificarSesion();
