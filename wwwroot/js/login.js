const formulario = document.getElementById("login-form");
const campoUsuario = document.getElementById("usuario");
const campoContrasena = document.getElementById("contrasena");
const botonIniciarSesion =
    document.getElementById("btn-iniciar-sesion");
const botonMostrarContrasena =
    document.getElementById("btn-mostrar-contrasena");
const mensajeLogin = document.getElementById("mensaje-login");

function mostrarMensaje(texto, tipo) {
    mensajeLogin.textContent = texto;
    mensajeLogin.className = `message ${tipo}`;
}

function limpiarMensaje() {
    mensajeLogin.textContent = "";
    mensajeLogin.className = "message";
}

async function comprobarSesionExistente() {
    try {
        const respuesta = await fetch("/api/auth/status", {
            credentials: "same-origin"
        });

        if (respuesta.ok) {
            window.location.replace("/estudiantes.html");
        }
    } catch {
        limpiarMensaje();
    }
}

botonMostrarContrasena.addEventListener("click", () => {
    const estaOculta = campoContrasena.type === "password";

    campoContrasena.type = estaOculta
        ? "text"
        : "password";

    botonMostrarContrasena.textContent = estaOculta
        ? "Ocultar"
        : "Mostrar";

    botonMostrarContrasena.setAttribute(
        "aria-label",
        estaOculta
            ? "Ocultar contraseña"
            : "Mostrar contraseña"
    );
});

formulario.addEventListener("submit", async event => {
    event.preventDefault();
    limpiarMensaje();

    if (!formulario.checkValidity()) {
        formulario.reportValidity();
        return;
    }

    botonIniciarSesion.disabled = true;
    botonIniciarSesion.textContent = "Ingresando...";

    const datos = {
        usuario: campoUsuario.value.trim(),
        contrasena: campoContrasena.value
    };

    try {
        const respuesta = await fetch("/api/auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "same-origin",
            body: JSON.stringify(datos)
        });

        const contenido = await respuesta
            .json()
            .catch(() => ({}));

        if (!respuesta.ok) {
            throw new Error(
                contenido.mensaje ??
                "No fue posible iniciar sesión."
            );
        }

        mostrarMensaje(
            contenido.mensaje ??
            "Inicio de sesión realizado correctamente.",
            "success"
        );

        window.setTimeout(() => {
            window.location.href = "/estudiantes.html";
        }, 500);
    } catch (error) {
        mostrarMensaje(
            error.message ??
            "Ocurrió un problema al iniciar sesión.",
            "error"
        );
    } finally {
        botonIniciarSesion.disabled = false;
        botonIniciarSesion.textContent = "Iniciar sesión";
    }
});

comprobarSesionExistente();
