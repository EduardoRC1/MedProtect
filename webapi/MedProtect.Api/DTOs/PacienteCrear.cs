namespace MedProtect.Api.DTOs;

public record PacienteCrear(
    string NombreCompleto,
    string? Sexo,
    DateOnly? FechaNacimiento,
    string? Telefono,
    string? Direccion,
    string? TipoSangre,
    string? Alergias,
    string CreadoPor
);