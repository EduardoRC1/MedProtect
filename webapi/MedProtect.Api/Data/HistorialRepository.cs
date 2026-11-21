
using Npgsql;
using NpgsqlTypes;
using MedProtect.Api.DTOs;
using MedProtect.Api.Models;

namespace MedProtect.Api.Data;

public class HistorialRepository
{
    private readonly NpgsqlDataSource _db;

    public HistorialRepository(NpgsqlDataSource db)
    {
        _db = db;
    }

    internal async Task<EntradaHistorial?> CrearAsync(
        Guid pacienteId,
        HistorialCrear request,
        CancellationToken ct)
    {
        const string sql = """
            INSERT INTO entrada_historial (
                paciente_id,
                descripcion,
                escrita_por
            )
            VALUES (
                @paciente_id,
                @descripcion,
                @escrita_por
            )
            RETURNING
                id, paciente_id, descripcion,
                escrita_utc, escrita_por;
            """;

        await using var cmd = _db.CreateCommand(sql);

        cmd.Parameters.AddWithValue(
            "paciente_id", NpgsqlDbType.Uuid, pacienteId);

        cmd.Parameters.AddWithValue(
            "descripcion", NpgsqlDbType.Text,
            request.Descripcion.Trim());

        cmd.Parameters.AddWithValue(
            "escrita_por", NpgsqlDbType.Text,
            request.EscritaPor.Trim());

        try
        {
            await using var reader =
                await cmd.ExecuteReaderAsync(ct);

            await reader.ReadAsync(ct);

            return new EntradaHistorial(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetString(2),
                reader.GetFieldValue<DateTime>(3),
                reader.GetString(4)
            );
        }
        catch (PostgresException ex) when (
            ex.SqlState == "23503")
        {
            return null;
        }
    }

    public async Task<List<EntradaHistorial>> ObtenerPorPacienteAsync(
        Guid pacienteId,
        CancellationToken ct)
    {
        const string sql = """
            SELECT
                id, paciente_id, descripcion,
                escrita_utc, escrita_por
            FROM entrada_historial
            WHERE paciente_id = @id
            ORDER BY escrita_utc DESC;
            """;

        await using var cmd = _db.CreateCommand(sql);

        cmd.Parameters.AddWithValue(
            "id", NpgsqlDbType.Uuid, pacienteId);

        await using var reader =
            await cmd.ExecuteReaderAsync(ct);

        var entradas = new List<EntradaHistorial>();

        while (await reader.ReadAsync(ct))
        {
            entradas.Add(new EntradaHistorial(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetString(2),
                reader.GetFieldValue<DateTime>(3),
                reader.GetString(4)
            ));
        }

        return entradas;
    }
}
