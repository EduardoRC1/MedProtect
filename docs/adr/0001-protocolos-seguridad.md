# ADR-{0001}: {protocolos-seguriada}

- **Estado:** {PROPUESTO_POR_ADR-Saul Adliel Garcia, Jhonatttan Alexis vecera, Jhoana gracia reinaga}
- **Fecha:** {15-11-2026}
- **Deciden:** {lalo} (Tech Lead) y {Jhoana gracia reinaga} (responsable de seguridad)
- **Amenaza que atiende:** {ID_DE_LA_AMENAZA}, de `docs/seguridad/modelo-de-amenazas.md`

## Contexto

{Al tratarse de un sistema de citas médicas, se maneja información altamente sensible y confidencial de los pacientes. Es imperativo asegurar que los expedientes y agendas solo sean accesibles por el personal correspondiente. Se requería un mecanismo sin estado (stateless) para la API que permitiera verificar la identidad de manera rápida y segura sin sobrecargar la memoria del servidor.}

## Opciones

1. **{Cookies}.** A favor: {Control total de la sesión: Puedes cerrar la sesión de un usuario al instante desde el servidor. Esto es vital si roban una cuenta o cambian la contraseña.}. En contra: {Limitadas a navegadores: No funcionan de forma nativa en aplicaciones móviles ni se comparten fácilmente entre distintos dominios.}.
2. **{Basic Authentication}.** A favor: {Extremadamente simple: No requiere librerías externas ni lógica compleja. Solo envía el usuario y la contraseña codificados en Base64 en cada petición.}. En contra: {Es inseguro para aplicaciones modernas web/móviles a menos que se implementen medidas extremas}.

## Decisión

{Se eligió utilizar JSON Web Tokens (JWT) para la autenticación y un modelo RBAC (Control de Acceso Basado en Roles) para la autorización, definiendo roles estrictos como: Paciente, Médico, Recepcionista y Administrador.}

## Consecuencias

- **A favor:** {Alta escalabilidad para el equipo de DevOps al mantener el servidor sin estado. Facilita la división de permisos y protege rutas críticas de la API.}
- **En contra:** {Si un token es robado antes de expirar, el atacante tiene acceso temporal garantizado. No se puede invalidar un JWT emitido fácilmente sin agregar complejidad (como una lista negra en base de datos).}
- **Queda abierto:** { Como Responsable de Seguridad, el reto será auditar al equipo de Desarrollo backend para asegurar que los tokens tengan tiempos de expiración muy cortos (ej. 15 minutos), exigir la rotación de tokens (Refresh Tokens) y coordinar pruebas de penetración (pentest) enfocadas en el robo de credenciales o elevación de privilegios.}

## Cumplimiento

Cómo se comprueba que se sigue respetando, y cómo se ve fallar:

```
{COMANDO_O_PRUEBA}
{SALIDA_QUE_DEMUESTRA_QUE_SE_CUMPLE}
```

---