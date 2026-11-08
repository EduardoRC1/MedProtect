# Checklist de la VM endurecida (hito H1)

- **Fecha:** {FECHA}
- **Conduce:** {DEVOPS} (DevOps). **Verifica:** {RESPONSABLE_SEGURIDAD} (responsable de seguridad)
- **Comandos:** `docs/seguridad/guia-endurecer-vm.md`, paso por paso.

**Cada punto con su salida pegada, o no cuenta.** Si no lo vieron fallar, no lo probaron.

> Repositorio público: tapen la IP y el nombre de la VM en lo que peguen (usen `ip-de-su-vm`). Ninguna llave privada ni contraseña.

## Resumen

| # | Control | Amenaza del modelo | Estado | Verificó |
|---|---|---|---|---|
| 1 | Cada integrante entra con su llave | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 2 | `deploy` existe, con llave propia; `sudo -l -U deploy` lo niega | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 3 | `deploy` escribe en `/opt/startup` y no en `/etc` | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 4 | `sshd -T`: `permitrootlogin no` y `passwordauthentication no` | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 5 | Contraseña rechazada: `Permission denied (publickey)` | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 6 | root rechazado | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 7 | `ufw status`: deny incoming y solo 22/tcp | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 8 | Señuelo 8081: contesta antes, expira después | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 9 | fail2ban: IP bloqueada, vista y desbloqueada | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 10 | Actualizaciones: activas, y resultado de `apt-get update` anotado | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |
| 11 | Tras reiniciar: todo sigue activo y la contraseña sigue rechazada | {ID_AMENAZA} | {ESTADO} | {INTEGRANTE} |

Estado: Pendiente, Verificado o Hallazgo (falló, y se anota abajo qué se hizo).

---

## 0. Línea base

Puertos antes de endurecer (`ss -ltn`):

```
{SALIDA}
```

## 1. Cada integrante entra con su llave

Huellas de `ssh-keygen -lf ~/.ssh/authorized_keys`, una por integrante (pueden recortar la huella, pero dejen el comentario con el nombre o correo):

```
{SALIDA}
```

Prueba de entrada sin contraseña, `ssh -o PasswordAuthentication=no equipo@ip-de-su-vm whoami`:

```
{SALIDA}
```

## 2. `deploy` existe y no tiene sudo

```
$ sudo -l -U deploy
{SALIDA}
```

## 3. `deploy` escribe en su carpeta y no en `/etc`

```
$ ssh -i ~/.ssh/deploy_startup deploy@ip-de-su-vm 'sudo -n true; touch /etc/prueba; touch /opt/startup/ok; ls /opt/startup'
{SALIDA}
```

## 4. El valor que ganó en SSH

```
$ sudo sshd -T | grep -Ei '^(permitroot|passwordauth|allowusers)'
{SALIDA}
```

## 5. Contraseña rechazada

```
$ ssh -o PubkeyAuthentication=no -o PreferredAuthentications=password equipo@ip-de-su-vm
{SALIDA}
```

## 6. root rechazado

```
$ ssh -o BatchMode=yes root@ip-de-su-vm
{SALIDA}
```

## 7. Firewall

```
$ sudo ufw status verbose
{SALIDA}
```

## 8. Señuelo en el 8081

Antes de activar UFW:

```
$ curl --max-time 3 -I http://ip-de-su-vm:8081/
{SALIDA}
```

Después de activar UFW:

```
$ curl --max-time 3 -I http://ip-de-su-vm:8081/
{SALIDA}
```

## 9. fail2ban

Configuración cargada:

```
$ sudo fail2ban-client get sshd maxretry
{SALIDA}
```

Después del ataque con `usuario-falso` (tapen la IP):

```
$ sudo fail2ban-client status sshd
{SALIDA}
```

Desbloqueo:

```
$ sudo fail2ban-client set sshd unbanip IP-BLOQUEADA
{SALIDA}
```

## 10. Actualizaciones automáticas

```
$ apt-config dump APT::Periodic::Unattended-Upgrade
{SALIDA}

$ sudo apt-get update
{SALIDA}
```

## 11. Después de reiniciar

```
$ diff ~/puertos-antes.txt ~/puertos-despues.txt
{SALIDA}

$ sudo ufw status | head -1
{SALIDA}

$ systemctl is-active fail2ban
{SALIDA}

$ sudo sshd -T | grep -E '^(permitrootlogin|passwordauthentication)'
{SALIDA}
```

Y la contraseña sigue rechazada después del reinicio:

```
{SALIDA}
```

---

## Hallazgos

Lo que falló o salió distinto, y qué se hizo. "Activo y sin alcanzar el espejo de actualizaciones" es un hallazgo válido.

| # | Qué pasó | Qué se hizo | Quién |
|---|---|---|---|
| {NUMERO_DEL_CONTROL} | {QUE_PASO} | {QUE_SE_HIZO} | {INTEGRANTE} |
