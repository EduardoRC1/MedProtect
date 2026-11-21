
using Npgsql;
using NpgsqlTypes;
using MedProtect.Api.DTOs;
using MedProtect.Api.Models;

namespace MedProtect.Api.Data;

public class PacienteRepository
{
    private readonly NpgsqlDataSource _db;

    public PacienteRepository(NpgsqlDataSource db)
    {
        _db = db;
    }

    internal async Task<Paciente> CrearAsync(
        PacienteCrear request,
        CancellationToken ct)
    {
        const string sql = """
            INSERT INTO paciente (
                nombre_completo,
                sexo,
                fecha_nacimiento,
                telefono,
                direccion,
                tipo_sangre,
                alergias,
                creado_por
            )
            VALUES (
                @nombre, @sexo, @fecha, @telefono,
                @direccion, @sangre, @alergias, @creado_por
            )
            RETURNING
                id, nombre_completo, sexo, fecha_nacimiento,
                telefono, direccion, tipo_sangre, alergias,
                creado_utc, creado_por;
            """;

        await using var cmd = _db.CreateCommand(sql);

        cmd.Parameters.AddWithValue(
            "nombre", NpgsqlDbType.Text,
            request.NombreCompleto.Trim());

        cmd.Parameters.AddWithValue(
            "sexo", NpgsqlDbType.Text,
            (object?)request.Sexo ?? DBNull.Value);

        cmd.Parameters.AddWithValue(
            "fecha", NpgsqlDbType.Date,
            (object?)request.FechaNacimiento ?? DBNull.Value);

        cmd.Parameters.AddWithValue(
            "telefono", NpgsqlDbType.Text,
            (object?)request.Telefono ?? DBNull.Value);

        cmd.Parameters.AddWithValue(
            "direccion", NpgsqlDbType.Text,
            (object?)request.Direccion ?? DBNull.Value);

        cmd.Parameters.AddWithValue(
            "sangre", NpgsqlDbType.Text,
            (object?)request.TipoSangre ?? DBNull.Value);

        cmd.Parameters.AddWithValue(
            "alergias", NpgsqlDbType.Text,
            (object?)request.Alergias ?? DBNull.Value);

        cmd.Parameters.AddWithValue(
            "creado_por", NpgsqlDbType.Text,
            request.CreadoPor.Trim());

        await using var reader =
            await cmd.ExecuteReaderAsync(ct);

        await reader.ReadAsync(ct);

        return new Paciente(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.IsDBNull(6) ? null : reader.GetString(6),
            reader.IsDBNull(7) ? null : reader.GetString(7),
            reader.GetFieldValue<DateTime>(8),
            reader.GetString(9)
        );
    }

    public async Task<Paciente?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken ct)
    {
        const string sql = """
            SELECT
                id, nombre_completo, sexo, fecha_nacimiento,
                telefono, direccion, tipo_sangre, alergias,
                creado_utc, creado_por
            FROM paciente
            WHERE id = @id;
            """;

        await using var cmd = _db.CreateCommand(sql);

        cmd.Parameters.AddWithValue(
            "id", NpgsqlDbType.Uuid, id);

        await using var reader =
            await cmd.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            return null;

        return new Paciente(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.IsDBNull(6) ? null : reader.GetString(6),
            reader.IsDBNull(7) ? null : reader.GetString(7),
            reader.GetFieldValue<DateTime>(8),
            reader.GetString(9)
        );
    }
}
