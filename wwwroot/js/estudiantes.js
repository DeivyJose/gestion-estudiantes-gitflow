"use strict";

/* =========================================================
   REFERENCIAS DE LA INTERFAZ
   ========================================================= */

const formularioEstudiante =
    document.getElementById("formulario-estudiante");

const campoEstudianteId =
    document.getElementById("estudiante-id");

const campoMatricula =
    document.getElementById("matricula");

const campoNombres =
    document.getElementById("nombres");

const campoApellidos =
    document.getElementById("apellidos");

const campoCorreo =
    document.getElementById("correo");

const campoCarrera =
    document.getElementById("carrera");

const campoFechaNacimiento =
    document.getElementById("fecha-nacimiento");

const campoActivo =
    document.getElementById("activo");

const grupoActivo =
    document.getElementById("grupo-activo");

const tituloFormulario =
    document.getElementById("titulo-formulario");

const modoFormulario =
    document.getElementById("modo-formulario");

const botonGuardar =
    document.getElementById("btn-guardar-estudiante");

const botonCancelarEdicion =
    document.getElementById("btn-cancelar-edicion");

const botonLimpiarFormulario =
    document.getElementById("btn-limpiar-formulario");

const mensajeFormulario =
    document.getElementById("mensaje-formulario");

const mensajeGlobal =
    document.getElementById("mensaje-global");

const campoBusqueda =
    document.getElementById("buscar-estudiante");

const botonLimpiarBusqueda =
    document.getElementById("btn-limpiar-busqueda");

const botonRecargar =
    document.getElementById("btn-recargar-estudiantes");

const cuerpoTabla =
    document.getElementById("cuerpo-tabla-estudiantes");

const tablaContenedor =
    document.querySelector(".table-wrapper");

const estadoCarga =
    document.getElementById("estado-carga");

const sinResultados =
    document.getElementById("sin-resultados");

const cantidadEstudiantes =
    document.getElementById("cantidad-estudiantes");

const modalEliminar =
    document.getElementById("modal-eliminar");

const nombreEstudianteEliminar =
    document.getElementById("nombre-estudiante-eliminar");

const botonConfirmarEliminacion =
    document.getElementById("btn-confirmar-eliminacion");

const botonCancelarEliminacion =
    document.getElementById("btn-cancelar-eliminacion");


/* =========================================================
   ESTADO DE LA PÁGINA
   ========================================================= */

let estudiantes = [];
let estudianteIdPendienteDeEliminar = null;


/* =========================================================
   FUNCIONES GENERALES
   ========================================================= */

function normalizarTexto(valor) {
    return String(valor ?? "")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase()
        .trim();
}

function mostrarMensaje(elemento, texto, tipo) {
    elemento.textContent = texto;
    elemento.className = `message ${tipo}`;
}

function limpiarMensaje(elemento) {
    elemento.textContent = "";
    elemento.className = "message";
}

function obtenerFechaLocalActual() {
    const fecha = new Date();

    fecha.setMinutes(
        fecha.getMinutes() - fecha.getTimezoneOffset()
    );

    return fecha.toISOString().split("T")[0];
}

function formatearFecha(fechaRecibida) {
    if (!fechaRecibida) {
        return "Sin fecha";
    }

    const fechaSinHora =
        String(fechaRecibida).split("T")[0];

    const partes = fechaSinHora.split("-");

    if (partes.length !== 3) {
        return fechaSinHora;
    }

    const [anio, mes, dia] = partes;

    return `${dia}/${mes}/${anio}`;
}

function obtenerMensajeDeError(contenido, mensajePredeterminado) {
    if (contenido?.mensaje) {
        return contenido.mensaje;
    }

    if (contenido?.errores) {
        const errores = Object.values(contenido.errores)
            .flat()
            .filter(Boolean);

        if (errores.length > 0) {
            return errores.join(" ");
        }
    }

    return mensajePredeterminado;
}


/* =========================================================
   COMUNICACIÓN CON LA API
   ========================================================= */

async function solicitarApi(url, opciones = {}) {
    const encabezados = {
        Accept: "application/json",
        ...(opciones.body
            ? { "Content-Type": "application/json" }
            : {}),
        ...(opciones.headers ?? {})
    };

    const respuesta = await fetch(url, {
        ...opciones,
        headers: encabezados,
        credentials: "same-origin"
    });

    if (respuesta.status === 401) {
        window.location.replace("/");
        throw new Error(
            "La sesión terminó. Debes iniciar sesión nuevamente."
        );
    }

    let contenido = null;

    if (respuesta.status !== 204) {
        const textoRespuesta = await respuesta.text();

        if (textoRespuesta) {
            try {
                contenido = JSON.parse(textoRespuesta);
            } catch {
                contenido = {
                    mensaje: textoRespuesta
                };
            }
        }
    }

    if (!respuesta.ok) {
        throw new Error(
            obtenerMensajeDeError(
                contenido,
                "No fue posible completar la operación."
            )
        );
    }

    return contenido;
}


/* =========================================================
   CARGA Y FILTRADO DE ESTUDIANTES
   ========================================================= */

function mostrarEstadoDeCarga() {
    estadoCarga.classList.remove("hidden");
    tablaContenedor.classList.add("hidden");
    sinResultados.classList.add("hidden");

    estadoCarga.textContent = "Cargando estudiantes...";
}

function ocultarEstadoDeCarga() {
    estadoCarga.classList.add("hidden");
}

async function cargarEstudiantes() {
    limpiarMensaje(mensajeGlobal);
    mostrarEstadoDeCarga();

    botonRecargar.disabled = true;
    botonRecargar.textContent = "Recargando...";

    try {
        const respuesta =
            await solicitarApi("/api/Estudiantes");

        estudiantes = Array.isArray(respuesta)
            ? respuesta
            : [];

        aplicarFiltro();
    } catch (error) {
        estudiantes = [];
        cuerpoTabla.replaceChildren();

        cantidadEstudiantes.textContent = "0";
        tablaContenedor.classList.add("hidden");
        sinResultados.classList.remove("hidden");

        mostrarMensaje(
            mensajeGlobal,
            error.message ??
                "No fue posible cargar los estudiantes.",
            "error"
        );
    } finally {
        ocultarEstadoDeCarga();

        botonRecargar.disabled = false;
        botonRecargar.textContent = "Recargar";
    }
}

function aplicarFiltro() {
    const termino = normalizarTexto(campoBusqueda.value);

    const estudiantesFiltrados = estudiantes.filter(estudiante => {
        if (!termino) {
            return true;
        }

        const contenidoBusqueda = normalizarTexto([
            estudiante.matricula,
            estudiante.nombres,
            estudiante.apellidos,
            estudiante.correo,
            estudiante.carrera,
            estudiante.activo ? "activo" : "inactivo"
        ].join(" "));

        return contenidoBusqueda.includes(termino);
    });

    renderizarEstudiantes(estudiantesFiltrados);
}


/* =========================================================
   TABLA DE ESTUDIANTES
   ========================================================= */

function crearCelda(texto) {
    const celda = document.createElement("td");
    celda.textContent = texto ?? "";

    return celda;
}

function crearBotonAccion({
    texto,
    clase,
    accion,
    estudianteId
}) {
    const boton = document.createElement("button");

    boton.type = "button";
    boton.textContent = texto;
    boton.className = `action-button ${clase}`;
    boton.dataset.accion = accion;
    boton.dataset.estudianteId = estudianteId;

    boton.id =
        `btn-${accion}-estudiante-${estudianteId}`;

    return boton;
}

function crearFilaEstudiante(estudiante) {
    const fila = document.createElement("tr");

    fila.dataset.estudianteId = estudiante.id;
    fila.id = `fila-estudiante-${estudiante.id}`;

    fila.appendChild(
        crearCelda(estudiante.matricula)
    );

    const celdaNombre = document.createElement("td");
    const contenedorNombre = document.createElement("div");
    const nombreCompleto = document.createElement("strong");
    const fechaNacimiento = document.createElement("small");

    contenedorNombre.className = "student-name";

    nombreCompleto.textContent =
        `${estudiante.nombres} ${estudiante.apellidos}`;

    fechaNacimiento.textContent =
        `Nacimiento: ${formatearFecha(
            estudiante.fechaNacimiento
        )}`;

    contenedorNombre.append(
        nombreCompleto,
        fechaNacimiento
    );

    celdaNombre.appendChild(contenedorNombre);
    fila.appendChild(celdaNombre);

    fila.appendChild(
        crearCelda(estudiante.correo)
    );

    fila.appendChild(
        crearCelda(estudiante.carrera)
    );

    const celdaEstado = document.createElement("td");
    const estado = document.createElement("span");

    estado.className = estudiante.activo
        ? "status-badge active"
        : "status-badge inactive";

    estado.textContent = estudiante.activo
        ? "Activo"
        : "Inactivo";

    celdaEstado.appendChild(estado);
    fila.appendChild(celdaEstado);

    const celdaAcciones = document.createElement("td");
    const acciones = document.createElement("div");

    acciones.className = "table-actions";

    const botonEditar = crearBotonAccion({
        texto: "Editar",
        clase: "edit-button",
        accion: "editar",
        estudianteId: estudiante.id
    });

    const botonEliminar = crearBotonAccion({
        texto: "Eliminar",
        clase: "delete-button",
        accion: "eliminar",
        estudianteId: estudiante.id
    });

    acciones.append(
        botonEditar,
        botonEliminar
    );

    celdaAcciones.appendChild(acciones);
    fila.appendChild(celdaAcciones);

    return fila;
}

function renderizarEstudiantes(lista) {
    cuerpoTabla.replaceChildren();

    cantidadEstudiantes.textContent =
        String(lista.length);

    if (lista.length === 0) {
        tablaContenedor.classList.add("hidden");
        sinResultados.classList.remove("hidden");
        return;
    }

    tablaContenedor.classList.remove("hidden");
    sinResultados.classList.add("hidden");

    const fragmento = document.createDocumentFragment();

    lista.forEach(estudiante => {
        fragmento.appendChild(
            crearFilaEstudiante(estudiante)
        );
    });

    cuerpoTabla.appendChild(fragmento);
}


/* =========================================================
   FORMULARIO: CREAR Y ACTUALIZAR
   ========================================================= */

function estaEnModoEdicion() {
    return campoEstudianteId.value !== "";
}

function actualizarTextoBotonGuardar() {
    botonGuardar.textContent = estaEnModoEdicion()
        ? "Guardar cambios"
        : "Guardar estudiante";
}

function restablecerFormulario() {
    formularioEstudiante.reset();

    campoEstudianteId.value = "";
    campoActivo.checked = true;

    tituloFormulario.textContent =
        "Registrar estudiante";

    modoFormulario.textContent =
        "Nuevo registro";

    grupoActivo.classList.add("hidden");
    botonCancelarEdicion.classList.add("hidden");

    botonGuardar.disabled = false;

    actualizarTextoBotonGuardar();
    limpiarMensaje(mensajeFormulario);
}

function obtenerDatosFormulario() {
    const datos = {
        matricula: campoMatricula.value.trim(),
        nombres: campoNombres.value.trim(),
        apellidos: campoApellidos.value.trim(),
        correo: campoCorreo.value.trim(),
        carrera: campoCarrera.value.trim(),
        fechaNacimiento: campoFechaNacimiento.value
    };

    if (estaEnModoEdicion()) {
        datos.activo = campoActivo.checked;
    }

    return datos;
}

function validarCamposConEspacios() {
    const camposObligatorios = [
        campoMatricula,
        campoNombres,
        campoApellidos,
        campoCorreo,
        campoCarrera,
        campoFechaNacimiento
    ];

    const campoVacio = camposObligatorios.find(campo =>
        campo.value.trim() === ""
    );

    if (campoVacio) {
        mostrarMensaje(
            mensajeFormulario,
            "Completa todos los campos obligatorios.",
            "error"
        );

        campoVacio.focus();
        return false;
    }

    return true;
}

async function guardarEstudiante(evento) {
    evento.preventDefault();

    limpiarMensaje(mensajeFormulario);
    limpiarMensaje(mensajeGlobal);

    if (!validarCamposConEspacios()) {
        return;
    }

    if (!formularioEstudiante.checkValidity()) {
        formularioEstudiante.reportValidity();
        return;
    }

    const datos = obtenerDatosFormulario();
    const id = campoEstudianteId.value;
    const editando = estaEnModoEdicion();

    botonGuardar.disabled = true;
    botonGuardar.textContent = editando
        ? "Guardando cambios..."
        : "Registrando...";

    try {
        if (editando) {
            await solicitarApi(
                `/api/Estudiantes/${id}`,
                {
                    method: "PUT",
                    body: JSON.stringify(datos)
                }
            );

            restablecerFormulario();

            mostrarMensaje(
                mensajeGlobal,
                "El estudiante fue actualizado correctamente.",
                "success"
            );
        } else {
            await solicitarApi(
                "/api/Estudiantes",
                {
                    method: "POST",
                    body: JSON.stringify(datos)
                }
            );

            restablecerFormulario();

            mostrarMensaje(
                mensajeGlobal,
                "El estudiante fue registrado correctamente.",
                "success"
            );
        }

        await cargarEstudiantes();
    } catch (error) {
        mostrarMensaje(
            mensajeFormulario,
            error.message ??
                "No fue posible guardar el estudiante.",
            "error"
        );
    } finally {
        botonGuardar.disabled = false;
        actualizarTextoBotonGuardar();
    }
}

function iniciarEdicion(estudianteId) {
    const estudiante = estudiantes.find(
        elemento =>
            Number(elemento.id) === Number(estudianteId)
    );

    if (!estudiante) {
        mostrarMensaje(
            mensajeGlobal,
            "No fue posible encontrar el estudiante seleccionado.",
            "error"
        );

        return;
    }

    limpiarMensaje(mensajeFormulario);
    limpiarMensaje(mensajeGlobal);

    campoEstudianteId.value = estudiante.id;
    campoMatricula.value = estudiante.matricula;
    campoNombres.value = estudiante.nombres;
    campoApellidos.value = estudiante.apellidos;
    campoCorreo.value = estudiante.correo;
    campoCarrera.value = estudiante.carrera;

    campoFechaNacimiento.value =
        String(estudiante.fechaNacimiento)
            .split("T")[0];

    campoActivo.checked = estudiante.activo;

    tituloFormulario.textContent =
        "Actualizar estudiante";

    modoFormulario.textContent =
        `Editando ID ${estudiante.id}`;

    grupoActivo.classList.remove("hidden");
    botonCancelarEdicion.classList.remove("hidden");

    actualizarTextoBotonGuardar();

    formularioEstudiante.scrollIntoView({
        behavior: "smooth",
        block: "start"
    });

    campoMatricula.focus();
}


/* =========================================================
   ELIMINACIÓN DE ESTUDIANTES
   ========================================================= */

function abrirModalEliminacion(estudianteId) {
    const estudiante = estudiantes.find(
        elemento =>
            Number(elemento.id) === Number(estudianteId)
    );

    if (!estudiante) {
        mostrarMensaje(
            mensajeGlobal,
            "No fue posible encontrar el estudiante seleccionado.",
            "error"
        );

        return;
    }

    estudianteIdPendienteDeEliminar =
        Number(estudiante.id);

    nombreEstudianteEliminar.textContent =
        `${estudiante.nombres} ${estudiante.apellidos}`;

    modalEliminar.classList.remove("hidden");

    window.setTimeout(() => {
        botonCancelarEliminacion.focus();
    }, 50);
}

function cerrarModalEliminacion() {
    modalEliminar.classList.add("hidden");

    estudianteIdPendienteDeEliminar = null;

    botonConfirmarEliminacion.disabled = false;
    botonConfirmarEliminacion.textContent =
        "Sí, eliminar";
}

async function confirmarEliminacion() {
    if (estudianteIdPendienteDeEliminar === null) {
        return;
    }

    const id = estudianteIdPendienteDeEliminar;

    botonConfirmarEliminacion.disabled = true;
    botonConfirmarEliminacion.textContent =
        "Eliminando...";

    try {
        await solicitarApi(
            `/api/Estudiantes/${id}`,
            {
                method: "DELETE"
            }
        );

        cerrarModalEliminacion();

        if (
            Number(campoEstudianteId.value) ===
            Number(id)
        ) {
            restablecerFormulario();
        }

        mostrarMensaje(
            mensajeGlobal,
            "El estudiante fue eliminado correctamente.",
            "success"
        );

        await cargarEstudiantes();
    } catch (error) {
        cerrarModalEliminacion();

        mostrarMensaje(
            mensajeGlobal,
            error.message ??
                "No fue posible eliminar el estudiante.",
            "error"
        );
    }
}


/* =========================================================
   EVENTOS
   ========================================================= */

formularioEstudiante.addEventListener(
    "submit",
    guardarEstudiante
);

botonLimpiarFormulario.addEventListener(
    "click",
    () => {
        restablecerFormulario();
        campoMatricula.focus();
    }
);

botonCancelarEdicion.addEventListener(
    "click",
    () => {
        restablecerFormulario();
        campoMatricula.focus();
    }
);

campoBusqueda.addEventListener(
    "input",
    aplicarFiltro
);

botonLimpiarBusqueda.addEventListener(
    "click",
    () => {
        campoBusqueda.value = "";
        aplicarFiltro();
        campoBusqueda.focus();
    }
);

botonRecargar.addEventListener(
    "click",
    cargarEstudiantes
);

cuerpoTabla.addEventListener(
    "click",
    evento => {
        const boton = evento.target.closest(
            "button[data-accion]"
        );

        if (!boton) {
            return;
        }

        const estudianteId =
            boton.dataset.estudianteId;

        const accion =
            boton.dataset.accion;

        if (accion === "editar") {
            iniciarEdicion(estudianteId);
        }

        if (accion === "eliminar") {
            abrirModalEliminacion(estudianteId);
        }
    }
);

botonConfirmarEliminacion.addEventListener(
    "click",
    confirmarEliminacion
);

botonCancelarEliminacion.addEventListener(
    "click",
    cerrarModalEliminacion
);

modalEliminar.addEventListener(
    "click",
    evento => {
        if (evento.target === modalEliminar) {
            cerrarModalEliminacion();
        }
    }
);

document.addEventListener(
    "keydown",
    evento => {
        if (
            evento.key === "Escape" &&
            !modalEliminar.classList.contains("hidden")
        ) {
            cerrarModalEliminacion();
        }
    }
);


/* =========================================================
   INICIALIZACIÓN
   ========================================================= */

function inicializarModuloEstudiantes() {
    campoFechaNacimiento.max =
        obtenerFechaLocalActual();

    restablecerFormulario();
    cargarEstudiantes();
}

inicializarModuloEstudiantes();
