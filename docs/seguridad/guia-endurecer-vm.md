# Guía paso a paso: endurecer la VM

Los comandos de la práctica 2 de la sesión 2, en orden, con lo que debe salir en cada uno y cómo regresar si algo falla. La evidencia de cada paso se pega en `docs/seguridad/checklist-vm.md`.

- **Conduce:** {DEVOPS} (DevOps). **Verifica y junta evidencia:** {RESPONSABLE_SEGURIDAD}.
- **Sistema:** Ubuntu Server. Usuarios: `equipo` (administra, con sudo) y `deploy` (despliega, sin sudo).
- **Donde dice `ip-de-su-vm`** va la IP de su VM. **No la escriban en el repositorio:** es público.
- Nadie pega un comando que no pueda explicar.

---

## La regla de oro: dos terminales

**La Terminal A no se cierra hasta el paso 7.** Es su sesión abierta dentro de la VM, la que les permite corregir si algo los deja fuera.

1. En la Terminal A: editan y validan la sintaxis.
2. En la Terminal A: aplican (reinician o activan).
3. En una **Terminal B nueva**, desde su laptop: ¿entra? Si sí, siguen al próximo control. Si no, corrigen desde la A.

Si nadie tiene sesión abierta y quedaron fuera: levanten la mano. La consola de Proxmox es la salida de emergencia del laboratorio.

---

## Paso 0. Línea base (5 minutos)

```sh
# Terminal A: entren y no la cierren hasta el paso 7
ssh equipo@ip-de-su-vm

# Qué escucha hoy
ss -ltn | tee ~/puertos-antes.txt

# La contraseña del papel sigue sirviendo para sudo: cámbienla
passwd
```

```sh
# En su laptop: ¿guardaron la huella correcta?
ssh-keygen -lF ip-de-su-vm
```

Comparen la huella con la de su papel: es la defensa contra suplantar al servidor.

---

## Paso 1. Una llave por persona (10 minutos, cada integrante)

Antes de quitar la contraseña, **todos** tienen que entrar con su llave.

**macOS, Linux o Git Bash:**

```sh
ssh-copy-id -i ~/.ssh/id_ed25519.pub equipo@ip-de-su-vm

# Debe contestar "equipo" sin pedir contraseña
ssh -o PasswordAuthentication=no equipo@ip-de-su-vm whoami
```

**PowerShell** (no trae `ssh-copy-id`):

```powershell
type $env:USERPROFILE\.ssh\id_ed25519.pub | ssh equipo@ip-de-su-vm "mkdir -p ~/.ssh; chmod 700 ~/.ssh; tr -d '\r' >> ~/.ssh/authorized_keys; chmod 600 ~/.ssh/authorized_keys"
```

**En la Terminal A, ¿están todos?** Una huella por integrante:

```sh
ssh-keygen -lf ~/.ssh/authorized_keys
```

Si `sshd` rechaza una llave correcta, revisen permisos: con `~/.ssh` o `authorized_keys` abiertos a otros se niega a usarla y el log dice `bad ownership or modes`.

---

## Paso 2. El usuario `deploy` (10 minutos)

Puede poco, y hay que verlo.

**En la laptop de DevOps** (sin frase: la usará un proceso automático en la sesión 4; **la privada nunca va al repositorio**):

```sh
ssh-keygen -t ed25519 -f ~/.ssh/deploy_startup -C "deploy@startup"
cat ~/.ssh/deploy_startup.pub
```

**En la Terminal A:**

```sh
sudo adduser --disabled-password --gecos "" deploy
sudo install -d -m 700 -o deploy -g deploy /home/deploy/.ssh
sudo install -d -m 750 -o deploy -g deploy /opt/startup
echo 'PEGUEN-AQUI-LA-LLAVE-PUBLICA' | sudo tee /home/deploy/.ssh/authorized_keys
sudo chown deploy:deploy /home/deploy/.ssh/authorized_keys
sudo chmod 600 /home/deploy/.ssh/authorized_keys
```

**Véanlo fallar:**

```sh
# Terminal A
sudo -l -U deploy
```

Debe salir: `User deploy is not allowed to run sudo on ...`

```sh
# Desde la laptop de DevOps
ssh -i ~/.ssh/deploy_startup deploy@ip-de-su-vm 'sudo -n true; touch /etc/prueba; touch /opt/startup/ok; ls /opt/startup'
```

Debe salir:

```
sudo: a password is required
touch: cannot touch '/etc/prueba': Permission denied
ok
```

Dos fallas y un acierto: no tiene sudo, no escribe en `/etc` y sí escribe en su carpeta.

---

## Paso 3. SSH sin contraseña (15 minutos)

**Primero escriban y validen, sin aplicar.** En la Terminal A:

```sh
# ¿Qué trae ya? Fíjense si hay un 50-cloud-init.conf
ls /etc/ssh/sshd_config.d/

sudo tee /etc/ssh/sshd_config.d/01-endurecer.conf >/dev/null << 'EOF'
PermitRootLogin no
PasswordAuthentication no
KbdInteractiveAuthentication no
PubkeyAuthentication yes
MaxAuthTries 3
LoginGraceTime 60
AllowUsers equipo deploy
EOF

# Sintaxis, y el valor que de verdad ganó
sudo sshd -t && sudo sshd -T | grep -Ei '^(permitroot|passwordauth|allowusers)'
```

Debe salir:

```
permitrootlogin no
passwordauthentication no
allowusers equipo
allowusers deploy
```

- **Trampa 1, gana el primero:** en SSH vale el primer valor que se lee, y los archivos se leen por número. Por eso el suyo empieza con `01`: así le gana a cualquier `50-cloud-init.conf`. `sshd -t` revisa la sintaxis; `sshd -T` muestra el valor que ganó.
- **¿Falta alguien en `AllowUsers`?** Se queda fuera.

**Ahora sí, apliquen** (Terminal A):

```sh
sudo sshd -t && sudo systemctl restart ssh
systemctl is-active ssh
```

Debe salir `active`.

- **Trampa 2, editar no es aplicar:** el archivo puede decir "no" y la contraseña seguir entrando hasta que reinician `ssh`. Reiniciar no corta las sesiones abiertas: por eso su Terminal A sigue viva.

**Véanlo fallar desde una Terminal B, en su laptop:**

```sh
# 1. Con contraseña: debe rechazarse
ssh -o PubkeyAuthentication=no -o PreferredAuthentications=password equipo@ip-de-su-vm
# Permission denied (publickey).

# 2. Como root: debe rechazarse
ssh -o BatchMode=yes root@ip-de-su-vm
# Permission denied (publickey).

# 3. Con su llave: debe entrar
ssh equipo@ip-de-su-vm whoami
# equipo
```

`(publickey)` dice que ya no se ofrece la contraseña; `(publickey,password)` significaría que sigue activa. Capturen los tres resultados.

**Si algo sale mal**, desde la Terminal A:

```sh
sudo rm /etc/ssh/sshd_config.d/01-endurecer.conf
sudo systemctl restart ssh
```

---

## Paso 4. UFW (10 minutos)

Primero la regla, luego el señuelo, luego activar. En la Terminal A:

```sh
# 1. Política y regla, todavía sin activar
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow 22/tcp
sudo ufw show added

# 2. Señuelo: un servidor web por 5 minutos
timeout 300 python3 -m http.server 8081 >/dev/null 2>&1 &
```

Desde su laptop, en una Terminal B:

```sh
# 3. Antes de activar: debe contestar
curl --max-time 3 -I http://ip-de-su-vm:8081/
# HTTP/1.0 200 OK
```

De vuelta en la Terminal A:

```sh
# 4. Ahora sí: contesten "y"
sudo ufw enable
sudo ufw status verbose
```

Desde su laptop:

```sh
# 5. Después de activar: debe expirar
curl --max-time 3 -I http://ip-de-su-vm:8081/
# curl: (28) Connection timed out after 3004 milliseconds

# 6. Y SSH debe seguir abierto
ssh equipo@ip-de-su-vm whoami
# equipo
```

En PowerShell usen `curl.exe`: `curl` solo es otro comando.

- **Por qué probar antes:** si solo prueban después, un timeout puede ser cualquier cosa (la red, una IP mal escrita). Viéndolo pasar y luego no pasar, fue UFW.
- **Trampa 3, UFW sin el 22:** si activan sin permitir el 22, la Terminal A sigue viva y todo parece bien, pero la siguiente conexión ya no entra. Si les pasa, desde la Terminal A: `sudo ufw allow 22/tcp`, y si no basta, `sudo ufw disable`.

---

## Paso 5. fail2ban (15 minutos)

**Instalar y configurar** (Terminal A):

```sh
sudo apt-get install -y fail2ban

sudo tee /etc/fail2ban/jail.local >/dev/null << 'EOF'
[DEFAULT]
bantime = 10m
findtime = 10m
maxretry = 5
backend = systemd

[sshd]
enabled = true
EOF

sudo systemctl restart fail2ban
sleep 3   # tarda en abrir su socket
sudo fail2ban-client get sshd maxretry
sudo fail2ban-client status sshd
```

Debe salir `5` y el estado de la jaula `sshd` con 0 bloqueados. Si ya estaba corriendo, sin el `restart` sigue con su configuración anterior. Se usa `jail.local` para no tocar `jail.conf`, que el paquete puede reemplazar.

**Provóquenlo:** un integrante ataca y otro lo ve desde la Terminal A.

```sh
# Terminal A: ¿salen con IP distinta?
ss -tn state established '( sport = :22 )'
```

El bloqueo es por IP y también corta la sesión abierta. **Si todos salen con la misma IP, avisen al instructor antes de atacar.**

En la laptop que ataca, que no es la de la Terminal A:

```sh
# macOS, Linux o Git Bash
for i in 1 2 3 4 5 6; do ssh -o BatchMode=yes usuario-falso@ip-de-su-vm true; done
```

```powershell
# PowerShell
1..6 | % { ssh -o BatchMode=yes usuario-falso@ip-de-su-vm true }
```

En la laptop que atacó debe salir varias veces `Permission denied (publickey).` y al final `Connection refused`. En la Terminal A:

```sh
sudo fail2ban-client status sshd
# Currently banned: 1, con la IP de la laptop que atacó

# Y se levanta el bloqueo
sudo fail2ban-client set sshd unbanip IP-BLOQUEADA
```

Se usa un **usuario falso** porque un usuario válido con llave equivocada no suma intentos. **Solo contra su propia VM y en este horario:** es lo que firmaron en el código de ética.

---

## Paso 6. Actualizaciones automáticas (5 minutos)

```sh
# 1. ¿Está activo? Debe decir "1"
apt-config dump APT::Periodic::Unattended-Upgrade

# 2. ¿Cuándo corre?
systemctl list-timers 'apt-daily*'

# 3. ¿Llega a algún espejo?
sudo apt-get update

# 4. ¿Qué instalaría?
sudo unattended-upgrade --dry-run --debug 2>&1 | grep -E 'Allowed origins|Packages that will'
```

**Así se ve fallar:** si la VM no llega al espejo, `apt-get update` avisa aunque termine sin error (`W: Failed to fetch ...`, `W: Some index files failed to download`). "Activo y sin alcanzar el espejo" es un hallazgo válido: se escribe en el checklist y en el modelo.

Si falta `/etc/apt/apt.conf.d/20auto-upgrades`:

```sh
sudo dpkg-reconfigure -f noninteractive unattended-upgrades
```

---

## Paso 7. Antes y después, y reiniciar (15 minutos)

En la Terminal A:

```sh
ss -ltn | tee ~/puertos-despues.txt
diff ~/puertos-antes.txt ~/puertos-despues.txt
sudo ufw status numbered

# Hasta ahora se suelta la Terminal A
sudo reboot
```

`ss` casi no cambia: lo que cambió es **quién llega**. Por eso el señuelo se midió desde afuera.

Un minuto después, desde su laptop:

```sh
ssh equipo@ip-de-su-vm
sudo ufw status | head -1
# Status: active
systemctl is-active fail2ban
# active
sudo sshd -T | grep -E '^(permitrootlogin|passwordauthentication)'
# permitrootlogin no
# passwordauthentication no
```

Repitan la prueba de la contraseña rechazada (paso 3): **un control que se pierde al reiniciar no estaba puesto.** Si la VM no regresa en dos minutos, levanten la mano.

Al terminar, actualicen en el modelo de amenazas el estado de las amenazas de la VM: de "Mitigar" a "Verificado".

---

## Plan B

| Qué pasó | Qué hacer |
|---|---|
| SSH rechaza y la Terminal A sigue abierta | `sudo rm /etc/ssh/sshd_config.d/01-endurecer.conf` y `sudo systemctl restart ssh` |
| UFW corta y la Terminal A sigue abierta | `sudo ufw allow 22/tcp`, y si no basta, `sudo ufw disable` |
| Los bloqueó fail2ban | Desde otra IP: `sudo fail2ban-client set sshd unbanip IP`, o esperar 10 minutos |
| Nadie tiene sesión abierta | Levanten la mano: consola de Proxmox con el instructor |
| La VM quedó inservible | Se regresa al snapshot del inicio de la clase y se rehace con lo aprendido |
| Sin salida a internet | Los paquetes ya vienen en la plantilla: sigan, y anótenlo en el checklist |
