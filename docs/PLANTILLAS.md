# Plantillas y estructura del repositorio

MedProtect: **alta de pacientes y mini historial clínico**. Este archivo explica qué hay en el repositorio, qué plantillas llenan ustedes y qué se revisa en cada hito.

## Estructura

```
MedProtect/
  README.md                       Su acta constitutiva (sesión 1)
  compose.yaml                    Levanta web, api y db
  .env.example                    Variables de ejemplo; se copia a .env, que no se versiona
  webapi/                         Minimal API en .NET 10 (iniciada; las rutas del SRS responden 501)
  webclient/                      Front estático con nginx
  db/init/                        Esquema de la base: lo diseñan ustedes
  docs/
    PLANTILLAS.md               Este archivo
    arquitectura/README.md      Stack, imágenes y cómo se levanta
    srs/SRS.md                  Requisitos del producto
    adr/                        Decisiones: plantilla e índice
    seguridad/
        modelo-de-amenazas.md   Datos, diagrama, ASVS y amenazas con STRIDE
        guia-endurecer-vm.md    Comandos paso a paso para endurecer la VM
        checklist-vm.md         Los 11 controles, con su salida pegada
        politicas.md            De la política al comando, y cuentas con mínimo privilegio
  .github/pull_request_template.md
```

## Cómo se usan las plantillas

1. Todo lo que está entre llaves, como `{RESPONSABLE_SEGURIDAD}`, se reemplaza por el dato real y **se borran las llaves**. Para ver lo que falta: `grep -rn "{[A-Z_]*}" docs/`.
2. Los renglones marcados como **Ejemplo** vienen de la clase: los adaptan a su sistema o los borran.
3. Todo entra por **pull request**, revisado por otro integrante que no lo escribió.
4. **El repositorio es público.** Nada de direcciones IP, nombres de la VM, contraseñas, llaves privadas ni datos de pacientes reales. En la evidencia que peguen, cambien la IP por `ip-de-su-vm`.

## Qué es cada archivo y quién conduce

| Archivo | Qué es | Conduce | Revisa |
|---|---|---|---|
| `docs/seguridad/modelo-de-amenazas.md` | Datos, diagrama de flujo de datos, nivel de ASVS, requisitos y al menos diez amenazas con STRIDE | Responsable de seguridad | Tech Lead |
| `docs/adr/0000-plantilla.md` | Plantilla de ADR: se copia como `0001-titulo-corto.md` para cada decisión | Tech Lead | Responsable de seguridad |
| `docs/adr/README.md` | Índice de las decisiones del equipo | Tech Lead | Cualquiera |
| `docs/seguridad/guia-endurecer-vm.md` | Los comandos de la práctica 2, paso por paso | DevOps | Responsable de seguridad |
| `docs/seguridad/checklist-vm.md` | Los 11 controles de la VM, cada uno con su salida pegada | DevOps | Responsable de seguridad |
| `docs/seguridad/politicas.md` | De la política al comando, y las cuentas con mínimo privilegio | Responsable de seguridad | Tech Lead |
| `docs/srs/SRS.md` | Requisitos de alta e historial, con cómo se comprueba cada uno | Product Owner | Tech Lead y QA |
| `.github/pull_request_template.md` | Lo que pregunta cada pull request antes de revisarse | Tech Lead | Todos |

## Lo que se revisa en el hito H1 (sesión 2)

**Práctica 1, modelo y ADR:**

- [ ] Diagrama con fronteras marcadas, en `docs/seguridad/modelo-de-amenazas.md`.
- [ ] Diez amenazas o más, con las seis letras de STRIDE.
- [ ] Cada amenaza con decisión y cómo se ve fallar.
- [ ] Nivel de ASVS declarado y justificado.
- [ ] ADR en `docs/adr/`, con dos opciones reales.
- [ ] Pull request revisado por otro integrante.

**Práctica 2, VM endurecida:** los 11 puntos de `docs/seguridad/checklist-vm.md`, cada uno con su salida. Lo que no alcancen en clase es la tarea para la sesión 3.

**Tarea para la sesión 3:** terminar el checklist por pull request; leer *Hacking APIs*, cap. 3, y dev-notes: JWT; instalar OWASP ZAP o Burp Suite Community (en Windows, ZAP pide Java 17 o superior).

**Regla del curso:** si no lo vieron fallar, no lo probaron.

## Variables

| Variable | Qué va | Ejemplo |
|---|---|---|
| `{FECHA}` | Fecha en formato AAAA-MM-DD | 2026-11-08 |
| `{TECH_LEAD}`, `{RESPONSABLE_SEGURIDAD}`, `{DEVOPS}`, `{BACKEND}`, `{QA}`, `{PRODUCT_OWNER}` | Nombre y usuario de GitHub de quien tiene ese rol | Ana López (@analopez) |
| `{INTEGRANTE}` | Quien hizo o verificó ese punto | @analopez |
| `{NIVEL_ASVS}` | L1, L2 o L3 | L2 |
| `{ELEMENTO}` | Elemento del diagrama: entidad, proceso, almacén, flujo o frontera | Flujo web a API |
| `{LETRA}` | Una de S, T, R, I, D, E | I |
| `{IMPACTO}`, `{PROB}`, `{RIESGO}` | Alto, Medio o Bajo | Alto |
| `{DECISION}` | Mitigar, Eliminar o Aceptar | Mitigar |
| `{ESTADO}` | Pendiente, En curso, Verificado, o Aceptado (ADR-000N) | Verificado |
| `{SALIDA}` | La salida real del comando, pegada tal cual, con la IP tapada | `Permission denied (publickey).` |
| `{CAMPOS_DEL_ALTA}`, `{CAMPOS_DE_LA_ENTRADA}` | Los campos que decidan en el SRS | Nombre, fecha de nacimiento... |
