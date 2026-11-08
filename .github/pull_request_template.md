## Qué cambia

<!-- En una a tres líneas. -->

**Entregable:** <!-- por ejemplo: H1, modelo de amenazas / H1, checklist de la VM / código de la API -->

**Rol que lo hizo:** <!-- Tech Lead, responsable de seguridad, DevOps, backend, QA o Product Owner -->

## Por qué

<!-- Qué amenaza, requisito o ADR atiende: A-0X, RF-PAC-0X, ADR-000X. -->

## Cómo se comprueba

<!-- El comando o la prueba, y lo que debe salir. Si es un control de seguridad: cómo se vio fallar. -->

```
```

## Antes de pedir revisión

- [ ] Sin IP, nombres de la VM, contraseñas, llaves ni datos personales en el diff (el repositorio es público).
- [ ] Sin variables `{ASI}` sin llenar en lo que toca este pull request.
- [ ] Si es código: `docker compose up --build` levanta y `/api/health/ready` contesta 200.
- [ ] Si cambia un control o una amenaza: el modelo de amenazas está actualizado.
- [ ] La rama parte de `main` actualizado.

## Revisión

- [ ] Lo revisó otro integrante que no lo escribió: <!-- @usuario -->
