
-- Esquema de MedProtect
-- PostgreSQL 17

-- Tabla principal de pacientes
CREATE TABLE paciente (
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

-- Entradas del historial clínico
CREATE TABLE entrada_historial (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    paciente_id UUID NOT NULL
        REFERENCES paciente(id),

    descripcion TEXT NOT NULL,

    escrita_utc TIMESTAMPTZ NOT NULL DEFAULT now(),
    escrita_por TEXT NOT NULL
);

-- Índice para consultar el historial de cada paciente
CREATE INDEX ix_entrada_historial_paciente
ON entrada_historial (paciente_id, escrita_utc);
