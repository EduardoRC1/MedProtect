# Documentación de Variables y Diccionario de Datos — MedProtect.

## 1. Variables Generales del Proyecto y Despliegue

| Variable / Valor | Qué va / Descripción | Ejemplo |
|---|---|---|
| `2026-11-14` (`{FECHA_DOCUMENTACION}`) | Fecha de elaboración o actualización del documento (AAAA-MM-DD) | 2026-11-08 |
| `EDUARDO REYNAGA COBOS (@EduardoRC1)`, `{RESPONSABLE_SEGURIDAD}`, `{DEVOPS}`, `{BACKEND}`, `{QA}`, `{PRODUCT_OWNER}` | Nombre completo y usuario de GitHub de la persona asignada a cada rol | Ana López (@analopez) |
| `LUIS ANGEL ALANIS ARVIZU` (`{RESPONSABLE_DOCUMENTACION}`) | Persona que elaboró o verificó el apartado | @analopez |
| `L2` (`{NIVEL_ASVS}`) | Nivel de seguridad seleccionado del estándar ASVS (L1, L2 o L3) | L2 |
| `ALMACÉN DE DATOS` (`{ELEMENTO_DIAGRAMA}`) | Elemento representado en el diagrama: entidad, proceso, almacén de datos, flujo o frontera de confianza | Flujo web a API |
| `{LETRA}` / `{LETRA_STRIDE}` | Categoría de amenaza del modelo STRIDE: S, T, R, I, D o E | I |
| `{IMPACTO}`, `{PROB}`, `{RIESGO}` | Clasificación del impacto, probabilidad y nivel de riesgo: Alto, Medio o Bajo | Alto |
| `{DECISION}` | Acción determinada para tratar el riesgo: Mitigar, Eliminar o Aceptar | Mitigar |
| `{ESTADO}` | Estado de la actividad: Pendiente, En curso, Verificado o Aceptado (con referencia ADR-000N cuando corresponda) | Verificado |
| `{SALIDA}` | Resultado real del comando ejecutado, pegado tal cual y ocultando datos sensibles, como direcciones IP o credenciales | `Permission denied (publickey).` |
| `{CAMPOS_DEL_ALTA}` | Campos definidos para el registro/alta de pacientes en el SRS: ID, Nombre completo, Edad, Sexo, Fecha de nacimiento, Teléfono, Correo, Dirección, Tipo de sangre, Alergias, Fecha de registro | ID, Nombre completo, Edad, Sexo, Fecha de nacimiento, Teléfono, Dirección, Tipo de sangre, Alergias, Fecha de registro |
| `{CAMPOS_DE_LA_ENTRADA}` | Campos definidos para el registro de consultas e historial clínico en el SRS: Fecha de consulta, Médico responsable, Enfermedad, Síntomas, Diagnóstico, Tratamiento, Observaciones | Enfermedad, Síntomas, Diagnóstico |
| `EduardoRC1` (`{CUENTA_GITHUB}`) | Cuenta propietaria del proyecto MedProtect y del repositorio privado `despliegue` | analopez |
| `{NOMBRE_RUNNER}` | Nombre único asignado al runner de la máquina virtual | medprotect-vm |
| `{VERSION_RUNNER}`, `{SUMA_RUNNER}` | Versión del GitHub Actions Runner instalada en la VM y su valor SHA-256 obtenido de la página *New self-hosted runner* | 2.338.2 / SHA-256 verificado |

## 2. Variables de Acceso y Autenticación

| Variable / Campo | Descripción / Qué va | Módulo / Ejemplo |
|---|---|---|
| `{USUARIO}` | Nombre de usuario o credencial de acceso del personal autorizado | Doctor / Secretaria |
| `{CONTRASENA}` | Clave secreta utilizada para validar el rol y autenticar al usuario en el inicio de sesión. Debe almacenarse mediante un algoritmo seguro de hash, nunca en texto plano ni versionarse en el repositorio | `********` (No versionar / No se muestra) |
| `{ROL_USUARIO}` | Función y permisos asignados al usuario dentro del sistema | Administrador / Doctor / Secretaria |
| `{ESTADO_CUENTA}` | Estado actual de la cuenta de usuario | Activa / Inactiva / Bloqueada |
| `{FECHA_CREACION_CUENTA}` | Fecha de creación de la cuenta del usuario | `2026-11-14` |
| `{ULTIMO_ACCESO}` | Fecha y hora del último inicio de sesión registrado | `2026-11-14 10:30:00` |
| `{INTENTOS_FALLIDOS}` | Número de intentos consecutivos de inicio de sesión fallidos | `3` |

## 3. Variables de la Ficha de Datos del Paciente

| Variable / Campo | Descripción / Qué va | Módulo / Ejemplo |
|---|---|---|
| `{ID}` | Identificador único del paciente dentro del sistema para búsqueda inmediata | `1042` / Ficha de datos |
| `{NOMBRE}` | Nombre completo del paciente | Juan Pérez García / Ficha de datos |
| `{FECHA_NACIMIENTO}` | Fecha de nacimiento del paciente (AAAA-MM-DD) | `1992-05-18` / Ficha de datos |
| `{EDAD}` | Edad actual del paciente en años, calculada a partir de su fecha de nacimiento | `34` / Ficha de datos |
| `{SEXO}` | Sexo biológico o género registrado del paciente, cuando sea necesario para la atención médica | Masculino / Femenino |
| `{TELEFONO}` | Número telefónico de contacto del paciente | `868-123-4567` / Ficha de datos |
| `{CORREO}` | Dirección de correo electrónico de contacto del paciente | `paciente@correo.com` / Ficha de datos |
| `{DIRECCION}` | Domicilio actual donde reside el paciente | Calle 5 #120, Col. Centro / Ficha de datos |
| `{FECHA_REGISTRO}` | Fecha en la que se da de alta / crea el expediente del paciente | `2026-11-14` / Ficha de datos |

## 4. Variables de Datos Clínicos

| Variable / Campo | Descripción / Qué va | Módulo / Ejemplo |
|---|---|---|
| `{TIPO_SANGRE}` | Grupo sanguíneo y factor Rh del paciente | `O+`, `A-` / Datos clínicos |
| `{ALERGIAS}` | Registro de alergias conocidas (medicamentos, alimentos u otras sustancias) | Penicilina / Datos clínicos |
| `{ANTECEDENTES_MEDICOS}` | Registro de enfermedades o padecimientos previos relevantes | Hipertensión / Datos clínicos |
| `{ENFERMEDAD}` | Padecimiento o enfermedad identificada y registrada durante la consulta | Hipertensión / Diabetes / Historial clínico |
| `{SINTOMAS}` | Sintomatología reportada por el paciente durante la consulta | Cefalea, fiebre / Historial clínico |
| `{DIAGNOSTICO}` | Evaluación clínica realizada por el médico responsable | Infección respiratoria / Historial clínico |
| `{TRATAMIENTO}` | Tratamiento o indicaciones médicas establecidas | Medicamento e indicaciones / Historial clínico |
| `{OBSERVACIONES}` | Notas adicionales relacionadas con la consulta médica | Recomendaciones y seguimiento / Historial clínico |
| `{FECHA_CONSULTA}` | Fecha en la que se realizó la consulta médica | `2026-11-14` / Historial clínico |
| `{ID_MEDICO}` | Identificador del médico responsable de la consulta | `DOC001` / Historial clínico |

## 5. Variables de Búsqueda y Gestión de Pacientes

| Variable / Campo | Descripción / Qué va | Módulo / Ejemplo |
|---|---|---|
| `{BUSCAR_PACIENTE}` | Consulta por nombre completo o ID único; retorna expediente o alerta `"Paciente no encontrado"` | Módulo de búsqueda |
| `{RESULTADO_BUSQUEDA}` | Resultado obtenido al realizar la búsqueda del paciente | Expediente encontrado |
| `{MENSAJE_ERROR}` | Mensaje mostrado cuando no se encuentra un paciente o ocurre un error | Paciente no encontrado |
| `{CONSULTAR_EXPEDIENTE}` | Permite acceder al expediente clínico de un paciente según los permisos del usuario | Módulo de expedientes |
| `{ACTUALIZAR_DATOS}` | Permite modificar los datos del paciente cuando el usuario tiene autorización | Módulo de gestión |
| `{REGISTRAR_CONSULTA}` | Permite agregar una nueva consulta al historial clínico del paciente | Módulo de historial clínico |

## 6. Consideraciones de Seguridad y Protección de Datos

- Los datos personales y clínicos deben estar protegidos contra accesos no autorizados.
- Las contraseñas nunca deben almacenarse en texto plano ni versionarse en el repositorio.
- Cada usuario debe acceder únicamente a los módulos y datos permitidos según su rol.
- Las consultas y modificaciones de expedientes deben quedar registradas en una bitácora de auditoría.
- Los datos clínicos deben ser accesibles únicamente para el personal autorizado.
- Las credenciales, claves secretas y archivos de configuración sensibles (`.env`) no deben publicarse en GitHub.
- Los datos de prueba deben ser ficticios y no contener información médica real de pacientes.
- La información del paciente debe almacenarse y transmitirse utilizando mecanismos de protección adecuados.
- Los datos personales y clínicos deben manejarse conforme a la legislación aplicable en materia de protección de datos personales (LFPDPPP) y los controles de seguridad establecidos (NIST CSF 2.0 / ASVS L2).