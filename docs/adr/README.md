# Decisiones de arquitectura y seguridad (ADR)

Un ADR guarda **el porqué** que en seis meses nadie va a recordar: contexto, opciones, decisión, consecuencias y cómo se comprueba. Plantilla: `0000-plantilla.md`.

## Reglas

- **Un archivo por decisión:** `NNNN-titulo-corto.md`, numerados en orden: `0001`, `0002`...
- **No se edita para cambiar de opinión:** se escribe uno nuevo que lo reemplaza, y el viejo cambia su estado.
- **Al menos dos opciones reales** en cada uno.
- **Toda amenaza que se acepta** (decisión "Aceptar" en el modelo) lleva un ADR con dueño, razón y fecha de revisión.
- Entra por pull request, revisado por otro integrante.

## Índice

| ADR | Título | Estado | Fecha | Amenaza que atiende |
|---|---|---|---|---|
| [ADR-{NUMERO}]({NUMERO}-{titulo-corto}.md) | {TITULO} | {ESTADO} | {FECHA} | {ID_AMENAZA} |

## Temas para su primer ADR (sesión 2)

- Cómo se entra al servidor.
- Qué nivel de ASVS persiguen.
- Qué puertos se exponen y por qué.
- Qué hacer con TLS en una red aislada.
- Dónde viven los secretos.

## Lo que ya viene decidido

El stack del proyecto (minimal API en .NET 10, front estático, PostgreSQL 17 e imágenes mínimas sin shell) lo fijó el instructor al iniciar el repositorio. Está descrito en `docs/arquitectura/README.md` y **no cuenta como ADR del equipo**: sus decisiones son las que toman ustedes.
