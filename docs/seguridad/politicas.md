# Políticas de seguridad de MedProtect

- **Aprueba:** {TECH_LEAD} (Tech Lead)
- **Mantiene:** {RESPONSABLE_SEGURIDAD} (responsable de seguridad)
- **Última revisión:** {FECHA}

Una política sin comando es un deseo, y un comando sin política, una ocurrencia. Cada política baja a un estándar medible, a un procedimiento paso a paso y a la evidencia que la comprueba.

> Un ADR guarda una decisión con su fecha. La política es la regla que sigue vigente y se revisa. En una empresa les van a pedir las dos.

## De la política al comando

| Política: qué y por qué | Estándar: con qué medida | Procedimiento: paso a paso | Evidencia: cómo se comprueba |
|---|---|---|---|
| **Ejemplo de la clase:** nadie entra a un servidor con contraseña | SSH solo con llave, una por persona | `01-endurecer.conf`, validar, reiniciar y probar (guía, paso 3) | `Permission denied (publickey)` en el checklist, punto 5 |
| {POLITICA} | {ESTANDAR} | {PROCEDIMIENTO} | {EVIDENCIA} |
| {POLITICA} | {ESTANDAR} | {PROCEDIMIENTO} | {EVIDENCIA} |
| {POLITICA} | {ESTANDAR} | {PROCEDIMIENTO} | {EVIDENCIA} |

Ideas para sus políticas: quién puede ver un historial clínico, dónde viven los secretos, qué puertos se exponen, cada cuánto se aplican parches, qué se registra en las bitácoras.

## Cuentas del servidor: mínimo privilegio

Cada cuenta con lo que necesita para su tarea, y nada más.

| Cuenta | Para qué existe | Entra por SSH | sudo |
|---|---|---|---|
| `root` | Es el sistema | Nunca | No aplica |
| `equipo` | Administrar la VM | Con la llave de cada integrante | Sí |
| `deploy` | Desplegar, solo en `/opt/startup` | Con su propia llave | No |
| Del servicio | Correr la aplicación (contenedores, sesiones 3 a 5) | No | No |
| {OTRA_CUENTA} | {PARA_QUE} | {COMO_ENTRA} | {SUDO} |

**Si roban una llave:** con la de `deploy`, un atacante puede tocar la aplicación; con la de `equipo`, todo el servidor. El daño lo decide el diseño de hoy.

## Cuentas de la aplicación

Quién puede hacer qué dentro de MedProtect. Se llena con el SRS (`docs/srs/SRS.md`).

| Rol | Dar de alta pacientes | Ver un paciente | Escribir en el historial | Ver el historial |
|---|---|---|---|---|
| {ROL} | {SI_NO} | {SI_NO} | {SI_NO} | {SI_NO} |
| {ROL} | {SI_NO} | {SI_NO} | {SI_NO} | {SI_NO} |
