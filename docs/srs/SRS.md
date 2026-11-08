# Especificación de Requisitos de Software
## MedProtect: alta de pacientes y mini historial clínico

- **Versión:** 0.1, borrador
- **Preparado por:** {PRODUCT_OWNER} (Product Owner), con {TECH_LEAD} y {RESPONSABLE_SEGURIDAD}
- **Fecha:** {FECHA}

> Cómo se llena: cada variable entre llaves se reemplaza. Los requisitos ya traen su identificador y su forma; ustedes deciden el contenido. **Un requisito que nadie puede reprobar todavía no es un requisito:** cada uno lleva cómo se comprueba.

## Historial de revisiones

| Quién | Fecha | Qué cambió | Versión |
|---|---|---|---|
| {INTEGRANTE} | {FECHA} | Primer borrador | 0.1 |

---

## 1. Introducción

### 1.1 Propósito del documento

{PARA_QUE_SIRVE_ESTE_SRS_Y_QUIEN_LO_USA}

### 1.2 Alcance del producto

MedProtect da de alta pacientes y lleva un mini historial clínico de cada uno. {QUE_RESUELVE_EN_DOS_O_TRES_FRASES}

**Dentro del alcance:** alta de pacientes, consulta de un paciente, entradas del historial y su consulta, y las cuentas de quienes lo usan.

**Fuera del alcance:** {QUE_NO_HACE, por ejemplo citas, recetas, facturación o expediente clínico completo}.

### 1.3 Definiciones

| Término | Definición |
|---|---|
| Paciente | {DEFINICION} |
| Alta | {DEFINICION} |
| Historial clínico | {DEFINICION} |
| Entrada del historial | {DEFINICION} |
| {TERMINO} | {DEFINICION} |

### 1.4 Referencias

| Referencia | Para qué |
|---|---|
| OWASP ASVS 5.0.0 | Requisitos de seguridad verificables (ver `docs/seguridad/modelo-de-amenazas.md`) |
| LFPDPPP | Ley Federal de Protección de Datos Personales en Posesión de los Particulares: los datos de salud son sensibles |
| NOM-004-SSA3-2012, del expediente clínico | Referencia sobre qué lleva un expediente y cuánto se conserva; el mini historial no pretende cumplirla completa: documenten qué toman |
| {REFERENCIA} | {PARA_QUE} |

---

## 2. Panorama del producto

### 2.1 Perspectiva

Una aplicación web: un front estático (`webclient/`), una minimal API en .NET 10 (`webapi/`) y PostgreSQL 17, todo en contenedores en la VM del equipo. Detalle en `docs/arquitectura/README.md`.

### 2.2 Funciones principales

1. Dar de alta un paciente.
2. Consultar un paciente.
3. Agregar una entrada a su historial clínico.
4. Ver el historial de un paciente.
5. Entrar con una cuenta y un rol.

### 2.3 Restricciones

- **Datos ficticios:** ningún dato real de pacientes, ni en pruebas ni en capturas.
- **Repositorio público:** nada de secretos, IP ni datos personales en el repositorio.
- **Stack fijo:** .NET 10, PostgreSQL 17, imágenes mínimas sin shell (ver arquitectura).
- {OTRA_RESTRICCION}

### 2.4 Usuarios

| Rol | Qué hace en el sistema | Qué no debe poder hacer |
|---|---|---|
| {ROL, por ejemplo recepción} | {QUE_HACE} | {QUE_NO} |
| {ROL, por ejemplo médico} | {QUE_HACE} | {QUE_NO} |
| {ROL, por ejemplo administrador} | {QUE_HACE} | {QUE_NO} |

### 2.5 Supuestos y dependencias

- {SUPUESTO}

---

## 3. Requisitos

### 3.1 Interfaces

| Id | Requisito |
|---|---|
| INT-01 | La API expone sus rutas bajo `/api/` y responde en JSON |
| INT-02 | El front llega a la API por el mismo origen (nginx reenvía `/api/`) |
| {INT-03} | {REQUISITO} |

### 3.2 Funcionales

#### Pacientes (PAC)

| Id | Requisito | Criterio de aceptación | Prioridad |
|---|---|---|---|
| RF-PAC-01 | El sistema debe dar de alta un paciente con {CAMPOS_OBLIGATORIOS} | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | Alta |
| RF-PAC-02 | El sistema debe rechazar un alta duplicada, identificada por {CRITERIO_DE_DUPLICADO} | Dado un paciente ya registrado, cuando se da de alta otra vez, entonces {RESULTADO} | {PRIORIDAD} |
| RF-PAC-03 | El sistema debe validar {QUE_VALIDA} y explicar el error al usuario | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | {PRIORIDAD} |
| RF-PAC-04 | Un usuario con permiso debe poder consultar un paciente | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | {PRIORIDAD} |
| {RF-PAC-05} | {REQUISITO} | {CRITERIO} | {PRIORIDAD} |

#### Historial clínico (HIS)

| Id | Requisito | Criterio de aceptación | Prioridad |
|---|---|---|---|
| RF-HIS-01 | Un médico debe poder agregar una entrada al historial de un paciente con {CAMPOS_DE_LA_ENTRADA} | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | Alta |
| RF-HIS-02 | Cada entrada debe guardar quién la escribió y cuándo, sin que el usuario pueda cambiarlo | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | Alta |
| RF-HIS-03 | El historial de un paciente debe verse en orden cronológico | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | {PRIORIDAD} |
| RF-HIS-04 | Una entrada {SE_PUEDE_O_NO_SE_PUEDE_EDITAR}; si se corrige, {COMO_QUEDA_EL_RASTRO} | Dado {CONTEXTO}, cuando {ACCION}, entonces {RESULTADO} | {PRIORIDAD} |
| {RF-HIS-05} | {REQUISITO} | {CRITERIO} | {PRIORIDAD} |

#### Cuentas y acceso (CUE)

| Id | Requisito | Criterio de aceptación | Prioridad |
|---|---|---|---|
| RF-CUE-01 | Solo un usuario autenticado puede usar el sistema | Sin sesión, cualquier ruta de `/api/pacientes` responde 401 | Alta |
| RF-CUE-02 | Cada rol solo puede hacer lo de su tabla en `docs/seguridad/politicas.md` | Dado un usuario de {ROL}, cuando intenta {ACCION_NO_PERMITIDA}, entonces 403 | Alta |
| {RF-CUE-03} | {REQUISITO} | {CRITERIO} | {PRIORIDAD} |

### 3.3 Calidad del servicio

#### Seguridad (SEG)

Los requisitos de ASVS que eligieron en el modelo de amenazas, con su versión.

| Id | Requisito | ASVS | Se prueba en |
|---|---|---|---|
| SEG-01 | Un usuario solo ve los pacientes e historiales que su rol le permite, aunque cambie el id en la URL | v5.0.0-8.2.2 | Sesión 3 |
| SEG-02 | Controles contra fuerza bruta al entrar | v5.0.0-6.3.1 | Sesión 3 |
| SEG-03 | Segundo factor para entrar | v5.0.0-6.3.3 | Sesión 3 |
| SEG-04 | El despliegue no publica `.git` ni metadatos de control de versiones | v5.0.0-13.4.1 | Sesión 5 |
| SEG-05 | Se registra cada autenticación, exitosa o fallida | v5.0.0-16.3.1 | Sesión 5 |
| {SEG-06} | {REQUISITO} | {ASVS} | {SESION} |

#### Desempeño, disponibilidad y observabilidad

| Id | Requisito | Cómo se mide |
|---|---|---|
| RNF-01 | `/api/health` responde mientras la API está viva; `/api/health/ready`, solo si llega a la base | `curl` a las dos rutas, con la base apagada y encendida |
| {RNF-02} | {REQUISITO} | {COMO_SE_MIDE} |

### 3.4 Privacidad y cumplimiento

| Id | Requisito |
|---|---|
| PRI-01 | Los datos de salud se tratan como datos personales sensibles: {CONSENTIMIENTO_Y_AVISO_DE_PRIVACIDAD} |
| PRI-02 | Solo se piden los datos necesarios: {LISTA_Y_POR_QUE} |
| PRI-03 | {CUANTO_SE_CONSERVAN_Y_COMO_SE_BORRAN} |
| {PRI-04} | {REQUISITO} |

### 3.5 Diseño e implementación

| Id | Requisito |
|---|---|
| DIS-01 | Todo el SQL va parametrizado |
| DIS-02 | Ningún secreto en el repositorio: van en `.env`, que no se versiona |
| DIS-03 | Los contenedores corren sin root y sin shell, con el sistema de archivos de solo lectura |
| DIS-04 | Todo cambio entra por pull request revisado por otro integrante |
| {DIS-05} | {REQUISITO} |

---

## 4. Verificación

| Requisito | Método | Cómo se comprueba | Estado |
|---|---|---|---|
| RF-PAC-01 | Prueba | {PRUEBA} | {ESTADO} |
| RF-HIS-01 | Prueba | {PRUEBA} | {ESTADO} |
| RF-CUE-02 | Prueba | Un usuario de cada rol intenta lo que no le toca | {ESTADO} |
| SEG-01 | Prueba | Con la sesión de un usuario, pedir un paciente que no le corresponde cambiando el id | {ESTADO} |
| {REQUISITO} | {PRUEBA_DEMOSTRACION_O_INSPECCION} | {COMO} | {ESTADO} |

---

## 5. Apéndices

### 5.1 Decisiones pendientes

| # | Pregunta | Quién decide | Para cuándo |
|---|---|---|---|
| D1 | ¿Qué campos son obligatorios en el alta? | {INTEGRANTE} | {FECHA} |
| D2 | ¿Cómo se identifica un duplicado? | {INTEGRANTE} | {FECHA} |
| D3 | ¿Una entrada del historial se puede corregir, y cómo queda el rastro? | {INTEGRANTE} | {FECHA} |
| D4 | {PREGUNTA} | {INTEGRANTE} | {FECHA} |
