
"use strict";

const API = "/api";

const $ = (id) => document.getElementById(id);

// ------------------------------------------
// ESTADO DE API Y POSTGRESQL
// ------------------------------------------

async function revisarEstado() {
    try {
        const api = await fetch(`${API}/health`);

        $("estado-api").textContent =
            api.ok ? "ACTIVA" : "ERROR";
    } catch {
        $("estado-api").textContent = "DESCONECTADA";
    }

    try {
        const db = await fetch(`${API}/health/ready`);

        $("estado-db").textContent =
            db.ok ? "CONECTADA" : "DESCONECTADA";
    } catch {
        $("estado-db").textContent = "DESCONECTADA";
    }
}

// ------------------------------------------
// REGISTRAR PACIENTE
// ------------------------------------------

$("form-paciente").addEventListener(
    "submit",
    async (event) => {
        event.preventDefault();

        const formulario = event.target;
        const boton = formulario.querySelector(
            'button[type="submit"]'
        );

        const datos = Object.fromEntries(
            new FormData(formulario)
        );

        if (!datos.fechaNacimiento) {
            datos.fechaNacimiento = null;
        }

        boton.disabled = true;

        try {
            const respuesta = await fetch(
                `${API}/pacientes/`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(datos)
                }
            );

            if (!respuesta.ok) {
                throw new Error(
                    `Error HTTP ${respuesta.status}`
                );
            }

            const paciente = await respuesta.json();

            $("mensaje-paciente").textContent =
                `Paciente registrado. ID: ${paciente.id}`;

            $("paciente-id").value = paciente.id;

            formulario.reset();

        } catch (error) {
            $("mensaje-paciente").textContent =
                `No se pudo registrar: ${error.message}`;
        } finally {
            boton.disabled = false;
        }
    }
);

// ------------------------------------------
// CONSULTAR PACIENTE
// ------------------------------------------

$("buscar-paciente").addEventListener(
    "click",
    async () => {
        const id = $("paciente-id").value.trim();

        if (!id) {
            $("paciente-encontrado").textContent =
                "Ingresa el UUID del paciente.";
            return;
        }

        try {
            const respuesta = await fetch(
                `${API}/pacientes/${encodeURIComponent(id)}`
            );

            if (!respuesta.ok) {
                throw new Error(
                    respuesta.status === 404
                        ? "Paciente no encontrado"
                        : `Error HTTP ${respuesta.status}`
                );
            }

            const paciente = await respuesta.json();

            $("paciente-encontrado").textContent =
                `Paciente: ${paciente.nombreCompleto}`;

            await cargarHistorial(id);

        } catch (error) {
            $("paciente-encontrado").textContent =
                error.message;

            $("lista-historial").replaceChildren();
        }
    }
);

// ------------------------------------------
// REGISTRAR HISTORIAL
// ------------------------------------------

$("form-historial").addEventListener(
    "submit",
    async (event) => {
        event.preventDefault();

        const formulario = event.target;
        const id = $("paciente-id").value.trim();

        const datos = {
            descripcion: $("descripcion").value,
            escritaPor: $("escrita").value
        };

        const boton = formulario.querySelector(
            'button[type="submit"]'
        );

        boton.disabled = true;

        try {
            const respuesta = await fetch(
                `${API}/pacientes/${encodeURIComponent(id)}/historial`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(datos)
                }
            );

            if (!respuesta.ok) {
                throw new Error(
                    `Error HTTP ${respuesta.status}`
                );
            }

            $("mensaje-historial").textContent =
                "Entrada clínica guardada correctamente.";

            $("descripcion").value = "";

            await cargarHistorial(id);

        } catch (error) {
            $("mensaje-historial").textContent =
                `No se pudo guardar: ${error.message}`;
        } finally {
            boton.disabled = false;
        }
    }
);

// ------------------------------------------
// CONSULTAR HISTORIAL
// ------------------------------------------

async function cargarHistorial(id) {
    const lista = $("lista-historial");
    lista.replaceChildren();

    try {
        const respuesta = await fetch(
            `${API}/pacientes/${encodeURIComponent(id)}/historial`
        );

        if (!respuesta.ok) {
            throw new Error(
                `Error HTTP ${respuesta.status}`
            );
        }

        const entradas = await respuesta.json();

        if (entradas.length === 0) {
            lista.textContent =
                "El paciente no tiene entradas registradas.";
            return;
        }

        entradas.forEach((entrada) => {
            const elemento = document.createElement("li");

            const fecha = new Date(
                entrada.escritaUtc
            ).toLocaleString("es-MX");

            elemento.textContent =
                `${fecha} | ${entrada.escritaPor}: ${entrada.descripcion}`;

            lista.appendChild(elemento);
        });

    } catch (error) {
        lista.textContent =
            `No se pudo consultar el historial: ${error.message}`;
    }
}

// ------------------------------------------
// INICIALIZAR
// ------------------------------------------

revisarEstado();
