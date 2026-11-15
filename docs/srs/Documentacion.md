# Documentación de Variables y Diccionario de Datos — MedProtect 

## Variables Generales del Proyecto y Despliegue

| Variable | Qué va | Ejemplo |
|---|---|---|
| 2026-11-14 | Fecha en formato AAAA-MM-DD | 2026-11-08 |
| EDUARDO REYNAGA COBOS, `{RESPONSABLE_SEGURIDAD}`, `{DEVOPS}`, `{BACKEND}`, `{QA}`, `{PRODUCT_OWNER}` | Nombre y usuario de GitHub de quien tiene ese rol | Ana López (@analopez) |
| LUIS ANGEL ALANIS ARVIZU | Quien hizo o verificó ese punto | @analopez |
| L2 | Nivel de seguridad ASVS (L1, L2 o L3) | L2 |
| ALMACEN DE DATOS | Elemento del diagrama: entidad, proceso, almacén, flujo o frontera | Flujo web a API |
| `{LETRA}` | Una de S, T, R, I, D, E (Modelo STRIDE) | I |
| `{IMPACTO}`, `{PROB}`, `{RIESGO}` | Alto, Medio o Bajo | Alto |
| `{DECISION}` | Mitigar, Eliminar o Aceptar | Mitigar |
| `{ESTADO}` | Pendiente, En curso, Verificado, o Aceptado (ADR-000N) | Verificado |
| `{SALIDA}` | La salida real del comando, pegada tal cual, con la IP tapada | `Permission denied (publickey).` |
| `{CAMPOS_DEL_ALTA}` | Campos definidos para el alta de pacientes en el SRS | ID, Nombre completo, Edad, Sexo, Fecha de nacimiento, Teléfono, Dirección, Tipo de sangre, Alergias, Fecha de registro |
| `{CAMPOS_DE_LA_ENTRADA}` | Campos definidos para el historial clínico en el SRS | Enfermedad, Síntomas, Diagnóstico |
| `{CUENTA_GITHUB}` | La cuenta dueña de MedProtect y del repositorio privado `despliegue` | analopez |
| `{NOMBRE_RUNNER}` | Nombre único del runner de la VM | medprotect-vm |
| `{VERSION_RUNNER}`, `{SUMA_RUNNER}` | Versión del runner y su SHA-256, de la página *New self-hosted runner* | 2.338.2 |

## Variables de Acceso, Módulos y Ficha Clínica 

| Variable / Campo | Descripción / Qué va | Módulo / Ejemplo |
|---|---|---|
| `{USUARIO}` | Credencial de acceso del personal autorizado | Doctor / Secretaria |
| `{CONTRASENA}` | Clave secreta para validar el rol en el inicio de sesión | `********` (No versionar) |
| `{ID}` | Identificador único del paciente para búsqueda inmediata | `1042` / Ficha de datos |
| `{NOMBRE}` | Nombre completo del paciente | Juan Pérez García / Ficha de datos |
| `{EDAD}` | Edad actual del paciente en años | `34` / Ficha de datos |
| `{SEXO}` | Sexo biológico o género del paciente | Masculino / Femenino |
| `{FECHA_NACIMIENTO}` | Fecha de nacimiento del paciente (AAAA-MM-DD) | `1992-05-18` / Ficha de datos |
| `{TELEFONO}` | Número telefónico de contacto del paciente | `868-123-4567` / Ficha de datos |
| `{CORREO}` | Correo electrónico de contacto del paciente | `paciente@correo.com` / Ficha de datos |
| `{DIRECCION}` | Domicilio actual donde reside el paciente | Calle 5 #120, Col. Centro / Ficha de datos |
| `{TIPO_SANGRE}` | Grupo sanguíneo y factor Rh del paciente | `O+`, `A-` / Datos clínicos |
| `{ALERGIAS}` | Registro de alergias conocidas (medicamentos, alimentos, etc.) | Penicilina / Datos clínicos |
| `{ENFERMEDAD}` | Padecimiento o diagnóstico clínico registrado | Hipertensión / Historial clínico |
| `{SINTOMAS}` | Sintomatología reportada por el paciente en consulta | Cefalea, fiebre / Historial clínico |
| `{FECHA_REGISTRO}` | Fecha en la que se da de alta el expediente del paciente | `2026-11-14` / Ficha de datos |
| `{BUSCAR_PACIENTE}` | Consulta por nombre completo o ID; retorna expediente o alerta "Paciente no encontrado" | Módulo Búsqueda |