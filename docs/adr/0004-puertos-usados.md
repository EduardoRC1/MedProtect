# ADR-{0004}: {puertos-usados}
- **Estado:** {Propuesto_POR_Lalo_ADR-0004}
- **Fecha:** {25-11-2026}
- **Deciden:** {Lalo} (Tech Lead) y {Jhoana} (responsable de seguridad)
- **Amenaza que atiende:** {ID_1}, de `docs/seguridad/modelo-de-amenazas.md`

## Contexto

{• Puerto 22 (SSH): Necesario como único canal seguro de administración remota, despliegue y mantenimiento del servidor.
• Puertos 80 y 443 (HTTP/HTTPS): Obligatorios para recibir el tráfico web público de los usuarios y gestionar la redirección automática hacia conexiones cifradas mediante certificados TLS/SSL.
• Puerto 8080 (Aplicación / Backend / Proxy inverso): Requerido para alojar el servicio principal de la aplicación o API interna antes de ser expuesta a través del proxy inverso.
• Puerto 5432 (PostgreSQL): Indispensable para la comunicación con la base de datos relacional del sistema (restringido idealmente a tráfico interno o redes privadas).
• Puerto 5341 / Seq (Logs estructurados): Necesario para centralizar y consultar los registros de eventos y errores generados por la aplicación en tiempo real.
• Puerto 3000 / Grafana (Métricas y monitoreo): Requerido para visualizar el estado de salud, rendimiento y alertas de la infraestructura mediante tableros de control.}

## Opciones

1. **{Firewall perimetral + Reverse Proxy (Solo 22, 80 y 443 públicos; el resto interno).}.** A favor: {Reduce drásticamente la superficie de ataque al ocultar la base de datos (5432) y el backend (8080) en localhost/red privada, centraliza los certificados TLS en los puertos 80/443 y permite acceder a Grafana y Seq de forma segura mediante túneles SSH o subdominios protegidos con autenticación.}. En contra: { Añade una capa extra de configuración inicial (Nginx/Caddy/Traefik) y requiere que el equipo sepa usar túneles SSH o VPN para consultar herramientas internas.}.
2. **{Apertura directa de todos los puertos en el firewall del servidor.}.** A favor: {Implementación inmediata y sencilla sin necesidad de configurar proxies inversos ni enrutamiento interno; cada herramienta es accesible directamente por su IP y puerto.}. En contra: {Expone servicios críticos como PostgreSQL (5432), Seq y Grafana directamente a internet, aumentando el riesgo de ataques de fuerza bruta, robo de datos y tráfico sin cifrar (HTTP plano en 8080, Seq y Grafana).}
3. **{Arquitectura aislada detrás de una VPN privada (WireGuard / Tailscale) + Puertos web públicos (80/443)}.** A favor: {Es el enfoque más seguro porque ni siquiera el puerto 22 (SSH), ni Grafana, ni Seq, ni PostgreSQL son visibles desde internet; solo los usuarios conectados a la VPN tienen acceso administrativo y de observabilidad.}. En contra: { Requiere desplegar, mantener y distribuir credenciales de una red VPN adicional para todo el equipo técnico antes de poder administrar o monitorear el servidor.}.

## Decisión

{Se adopta la arquitectura de Firewall perimetral con Reverse Proxy (exponiendo únicamente los puertos 22, 80 y 443 a internet) porque garantiza el acceso público cifrado a la aplicación web, protege la base de datos (5432) y el backend (8080) aislándolos en la red interna, y permite consultar Seq y Grafana de forma segura mediante túneles SSH o rutas protegidas sin abrir puertos administrativos innecesarios.}

## Consecuencias

- **A favor:** {•  Se reduce la superficie de ataque pública a solo tres puertos esenciales (22, 80 y 443), protegiendo PostgreSQL (5432) y el backend (8080) de accesos externos directos. Además, se centraliza el cifrado TLS/HTTPS en el proxy inverso y se garantiza que tanto Seq como Grafana se consulten únicamente por canales seguros.}
- **En contra:** { Aumenta la complejidad de configuración inicial al tener que mantener reglas de firewall estrictas, certificados SSL y bloques de enrutamiento en el proxy inverso. También obliga a los desarrolladores y administradores a abrir túneles SSH (o conectarse a una red autorizada) cada vez que necesiten revisar logs en Seq, métricas en Grafana o conectarse a la base de datos.}
- **Queda abierto:** {La renovación automática de los certificados TLS (puerto 443), la rotación periódica de credenciales de Grafana, Seq y PostgreSQL, y el monitoreo de intentos de intrusión en el puerto 22 (como la configuración de Fail2Ban o bloqueo por IP), responsabilidades que quedarán a cargo del equipo de DevOps / Infraestructura.
}

## Cumplimiento

Cómo se comprueba que se sigue respetando, y cómo se ve fallar:

```
{# 1. Verificar en el firewall (UFW) que solo 22, 80 y 443 tienen permiso de entrada pública
sudo ufw status numbered | grep ALLOW

# Salida que demuestra que se cumple:
[ 1] 22/tcp                     ALLOW IN    Anywhere
[ 2] 80/tcp                     ALLOW IN    Anywhere
[ 3] 443/tcp                    ALLOW IN    Anywhere

# 2. Comprobar que los servicios internos solo escuchan en localhost (127.0.0.1) o red interna, nunca en 0.0.0.0
ss -tulnp | grep -E ":(5432|8080|5341|3000)\b"

# Salida que demuestra que se cumple:
tcp   LISTEN 0   128   127.0.0.1:5432    0.0.0.0:*   users:(("postgres",pid=1024,fd=5))
tcp   LISTEN 0   128   127.0.0.1:8080    0.0.0.0:*   users:(("dotnet",pid=1150,fd=12))
tcp   LISTEN 0   128   127.0.0.1:5341    0.0.0.0:*   users:(("seq",pid=1210,fd=8))
tcp   LISTEN 0   128   127.0.0.1:3000    0.0.0.0:*   users:(("grafana",pid=1305,fd=9))
}

{SALIDA_QUE_DEMUESTRA_QUE_SE_CUMPLE}
```
• En la primera prueba, ufw status muestra reglas ALLOW IN Anywhere para los puertos 5432, 8080, 5341 o 3000, o el firewall aparece como Status: inactive.
• En la segunda prueba, alguno de los puertos internos aparece escuchando en todas las interfaces (0.0.0.0:5432 o *:3000), lo que indica que un contenedor de Docker o un servicio saltó el aislamiento local y quedó expuesto directamente a internet.
---