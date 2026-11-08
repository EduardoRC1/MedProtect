// MedProtect API: alta de pacientes y mini historial clínico.
//
// Esqueleto inicial. Ya hace dos cosas: decir si está viva y si llega a PostgreSQL. Las rutas del SRS
// (docs/srs/SRS.md) ya existen y responden 501 "Por implementar" hasta que el equipo las construya,
// requisito por requisito, cada una con su prueba.
using Npgsql;

// La imagen es distroless: no tiene shell ni curl para el healthcheck del contenedor. La propia API se
// revisa a sí misma: `dotnet MedProtect.Api.dll --healthcheck` sale con 0 si /api/health responde.
if (args.Contains("--healthcheck"))
{
    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
    try
    {
        var respuesta = await http.GetAsync("http://127.0.0.1:8080/api/health");
        return respuesta.IsSuccessStatusCode ? 0 : 1;
    }
    catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
    {
        return 1;
    }
}

var builder = WebApplication.CreateBuilder(args);

// No anunciar el servidor en cada respuesta (cabecera Server: Kestrel).
builder.WebHost.ConfigureKestrel(opciones => opciones.AddServerHeader = false);

// La cadena de conexión llega por variable de entorno (ConnectionStrings__MedProtect, ver compose.yaml).
// Nunca en appsettings.json ni en el repositorio. Sin ella, la API no arranca: mejor fallar claro.
var cadena = builder.Configuration.GetConnectionString("MedProtect")
    ?? throw new InvalidOperationException("Falta ConnectionStrings__MedProtect. Revisen su archivo .env.");
builder.Services.AddSingleton(NpgsqlDataSource.Create(cadena));

// Errores al cliente en formato estándar (application/problem+json) y sin detalles internos.
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

var api = app.MapGroup("/api");

// Viva: el proceso responde. No toca la base: si la base cae, la API sigue viva.
api.MapGet("/health", () => Results.Ok(new { estado = "viva" }));

// Lista: además llega a PostgreSQL. No dice versiones ni nombres: eso sería regalar información.
api.MapGet("/health/ready", async (NpgsqlDataSource db, ILogger<Program> log, CancellationToken ct) =>
{
    try
    {
        await using var comando = db.CreateCommand("SELECT 1");
        await comando.ExecuteScalarAsync(ct);
        return Results.Ok(new { estado = "lista" });
    }
    catch (Exception e) when (e is NpgsqlException or TimeoutException)
    {
        log.LogWarning(e, "La base de datos no responde");
        return Results.Problem(title: "La base de datos no responde", statusCode: 503);
    }
});

// Rutas del SRS. Cada una responde 501 hasta que el equipo la implemente.
var pacientes = api.MapGroup("/pacientes");
pacientes.MapPost("/", PorImplementar("RF-PAC-01, dar de alta un paciente"));
pacientes.MapGet("/{id}", PorImplementar("RF-PAC-04, consultar un paciente"));
pacientes.MapPost("/{id}/historial", PorImplementar("RF-HIS-01, agregar una entrada al historial"));
pacientes.MapGet("/{id}/historial", PorImplementar("RF-HIS-03, ver el historial de un paciente"));

app.Run();
return 0;

static Delegate PorImplementar(string requisito) =>
    () => Results.Problem(title: "Por implementar", detail: $"{requisito}. Ver docs/srs/SRS.md.", statusCode: 501);
