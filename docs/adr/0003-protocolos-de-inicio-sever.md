# ADR-{0003}: {Como se inicia el servidor}

- **Estado:** {PROPUESTO_POR_Jhoana_ADR-0003}
- **Fecha:** {21-11-2026}
- **Deciden:** {Lalo} (Tech Lead) y {Jhoana gracia} (responsable de seguridad)
- **Amenaza que atiende:** {ID_1}, de `docs/seguridad/modelo-de-amenazas.md`

## Contexto

{Se decidio usar llaves SSH para cada persona por un sistema de vulnerabilidad en las contraseñas, tambien  permite administrar sistemas de forma remota a través de un canal cifrado, protegiendo las contraseñas, los comandos y los datos frente a espionaje o intercepciones}

## Opciones

1. **{SSH}.** A favor: {Es muy ligero, rápido, seguro gracias al cifrado y permite automatizar tareas con scripts o trabajar incluso con conexiones lentas. }. En contra: {1. Requiere conocer comandos de terminal y no ofrece interfaz gráfica para tareas visuales. }.
2. **{RDP / VNC / cPanel}.** A favor: { Es más intuitivo y visual, ideal para quienes prefieren usar ratón y menús sin memorizar comandos.}. En contra: {Consume más recursos del servidor y ancho de banda, y suele tener una mayor superficie de ataque si no se protege bien.}.

## Decisión

{Se eligio  SSH sobre cualquier otra alternativa porque es el estándar de la industria para administrar servidores de forma rápida, ligera y con el menor riesgo de seguridad posible.}

## Consecuencias

- **A favor:** { Se obtiene un canal de administración cifrado de extremo a extremo, ligero y con bajo consumo de recursos. Facilita la automatización de despliegues y tareas repetitivas mediante scripts, además de permitir la transferencia segura de archivos (SFTP/SCP) sin abrir puertos adicionales.}
- **En contra:** {Exige que el equipo técnico domine la línea de comandos y la gestión segura de claves criptográficas (generación, respaldo y rotación). Carece de interfaz gráfica nativa, lo que dificulta tareas visuales o el manejo de aplicaciones que requieren entorno de escritorio.}
- **Queda abierto:** { La gestión del ciclo de vida de los accesos (altas, bajas y auditoría de permisos de usuarios) y la protección perimetral del puerto (como configurar un firewall, Fail2Ban o una VPN/Bastion host para evitar ataques de fuerza bruta), temas que deberá resolver el equipo de DevOps, Seguridad o Administración de Sistemas (SysAdmin).}

## Cumplimiento

Cómo se comprueba que se sigue respetando, y cómo se ve fallar:

```
{# 1. Comprobar que el servicio SSH está activo, exige clave pública y prohíbe contraseñas o acceso root directo
sudo sshd -T | grep -E "permitrootlogin|passwordauthentication|pubkeyauthentication"

# Salida que demuestra que se cumple:
permitrootlogin no
passwordauthentication no
pubkeyauthentication yes

# 2. Comprobar que no hay puertos de protocolos inseguros abiertos (ej. Telnet en puerto 23 o FTP en puerto 21)
ss -tulnp | grep -E ":(21|23)\b" || echo "OK: Solo acceso seguro activo"
}
{# Salida que demuestra que se cumple:
OK: Solo acceso seguro activo}
```
---