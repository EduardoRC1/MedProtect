namespace MedProtect.Api.Models;

public record EntradaHistorial(
    Guid Id,
    Guid PacienteId,
    string Descripcion,
    DateTime EscritaUtc,
    string EscritaPor
);