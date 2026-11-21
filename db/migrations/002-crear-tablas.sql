
BEGIN;

CREATE TABLE IF NOT EXISTS paciente (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre_completo TEXT NOT NULL,
    sexo TEXT,
    fecha_nacimiento DATE,
    telefono TEXT,
    direccion TEXT,
    tipo_sangre TEXT,
    alergias TEXT,
    creado_utc TIMESTAMPTZ NOT NULL DEFAULT now(),
    creado_por TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS entrada_historial (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    paciente_id UUID NOT NULL REFERENCES paciente(id),
    descripcion TEXT NOT NULL,
    escrita_utc TIMESTAMPTZ NOT NULL DEFAULT now(),
    escrita_por TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_entrada_historial_paciente
ON entrada_historial (paciente_id, escrita_utc);

COMMIT;
