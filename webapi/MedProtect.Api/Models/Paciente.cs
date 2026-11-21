namespace MedProtect.Api.Models;

public record Paciente(
    Guid Id,
    string NombreCompleto,
    string? Sexo,
    DateOnly? FechaNacimiento,
    string? Telefono,
    string? Direccion,
    string? TipoSangre,
    string? Alergias,
    DateTime CreadoUtc,
    string CreadoPor
);