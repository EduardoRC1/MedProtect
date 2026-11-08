# ADR-{NUMERO}: {TITULO_CORTO_DE_LA_DECISION}

> **Cómo usar esta plantilla:** cópienla como `docs/adr/{NUMERO}-{titulo-corto}.md` (por ejemplo `0001-acceso-al-servidor.md`), llenen cada variable y agreguen la decisión al índice en `docs/adr/README.md`. Esta plantilla no se edita ni se numera.

- **Estado:** {PROPUESTO_ACEPTADO_O_REEMPLAZADO_POR_ADR-XXXX}
- **Fecha:** {FECHA}
- **Deciden:** {TECH_LEAD} (Tech Lead) y {RESPONSABLE_SEGURIDAD} (responsable de seguridad)
- **Amenaza que atiende:** {ID_DE_LA_AMENAZA}, de `docs/seguridad/modelo-de-amenazas.md`

## Contexto

{QUE_LO_OBLIGA}

Situación, restricciones y la amenaza de su modelo que atiende (A-0X). Qué pasa si no se decide nada.

## Opciones

1. **{OPCION_A}.** A favor: {PROS_A}. En contra: {CONTRAS_A}.
2. **{OPCION_B}.** A favor: {PROS_B}. En contra: {CONTRAS_B}.
3. **{OPCION_C_SI_LA_HAY}.** A favor: {PROS_C}. En contra: {CONTRAS_C}.

Al menos **dos opciones reales**: una que de verdad se pudo haber elegido, no una de relleno.

## Decisión

{LA_OPCION_ELEGIDA_EN_UNA_FRASE_COMPLETA_Y_POR_QUE}

## Consecuencias

- **A favor:** {LO_QUE_SE_GANA}
- **En contra:** {LO_QUE_CUESTA}
- **Queda abierto:** {LO_QUE_NO_RESUELVE_Y_QUIEN_LO_VA_A_VER}

## Cumplimiento

Cómo se comprueba que se sigue respetando, y cómo se ve fallar:

```
{COMANDO_O_PRUEBA}
{SALIDA_QUE_DEMUESTRA_QUE_SE_CUMPLE}
```

---

Un ADR no se edita para cambiar de opinión: se escribe otro que lo reemplaza, y este cambia su estado a "Reemplazado por ADR-XXXX".
