# syntax=docker/dockerfile:1
# Front de MedProtect. Se construye con la carpeta webclient/ como contexto (lo hace compose.yaml).
#
# nginx distroless de Chainguard: sin shell ni gestor de paquetes, y sin root. Chainguard publica gratis
# solo la etiqueta latest, reconstruida a diario con los parches de seguridad.
FROM cgr.dev/chainguard/nginx:latest
COPY nginx.conf /etc/nginx/conf.d/nginx.default.conf
COPY public/ /usr/share/nginx/html/
EXPOSE 8080
