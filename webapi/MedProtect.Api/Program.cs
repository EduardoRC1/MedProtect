// // MedProtect API: alta de pacientes y mini historial clínico.
// //
// // Esqueleto inicial. Ya hace dos cosas: decir si está viva y si llega a PostgreSQL. Las rutas del SRS
// // (docs/srs/SRS.md) ya existen y responden 501 "Por implementar" hasta que el equipo las construya,
// // requisito por requisito, cada una con su prueba.
// using Npgsql;

// // La imagen es distroless: no tiene shell ni curl para el healthcheck del contenedor. La propia API se
// // revisa a sí misma: `dotnet MedProtect.Api.dll --healthcheck` sale con 0 si /api/health responde.
// if (args.Contains("--healthcheck"))
// {
//     using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
//     try
//     {
//         var respuesta = await http.GetAsync("http://127.0.0.1:8080/api/health");
//         return respuesta.IsSuccessStatusCode ? 0 : 1;
//     }
//     catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
//     {
//         return 1;
//     }
// }

// var builder = WebApplication.CreateBuilder(args);

// // No anunciar el servidor en cada respuesta (cabecera Server: Kestrel).
// builder.WebHost.ConfigureKestrel(opciones => opciones.AddServerHeader = false);

// // La cadena de conexión llega por variable de entorno (ConnectionStrings__MedProtect, ver compose.yaml).
// // Nunca en appsettings.json ni en el repositorio. Sin ella, la API no arranca: mejor fallar claro.
// var cadena = builder.Configuration.GetConnectionString("MedProtect")
//     ?? throw new InvalidOperationException("Falta ConnectionStrings__MedProtect. Revisen su archivo .env.");
// builder.Services.AddSingleton(NpgsqlDataSource.Create(cadena));

// // Errores al cliente en formato estándar (application/problem+json) y sin detalles internos.
// builder.Services.AddProblemDetails();

// var app = builder.Build();
// app.UseExceptionHandler();

// var api = app.MapGroup("/api");

// // Viva: el proceso responde. No toca la base: si la base cae, la API sigue viva.
// api.MapGet("/health", () => Results.Ok(new { estado = "viva" }));

// // Lista: además llega a PostgreSQL. No dice versiones ni nombres: eso sería regalar información.
// api.MapGet("/health/ready", async (NpgsqlDataSource db, ILogger<Program> log, CancellationToken ct) =>
// {
//     try
//     {
//         await using var comando = db.CreateCommand("SELECT 1");
//         await comando.ExecuteScalarAsync(ct);
//         return Results.Ok(new { estado = "lista" });
//     }
//     catch (Exception e) when (e is NpgsqlException or TimeoutException)
//     {
//         log.LogWarning(e, "La base de datos no responde");
//         return Results.Problem(title: "La base de datos no responde", statusCode: 503);
//     }
// });

// // Rutas del SRS. Cada una responde 501 hasta que el equipo la implemente.
// var pacientes = api.MapGroup("/pacientes");
// pacientes.MapPost("/", PorImplementar("RF-PAC-01, dar de alta un paciente"));
// pacientes.MapGet("/{id}", PorImplementar("RF-PAC-04, consultar un paciente"));
// pacientes.MapPost("/{id}/historial", PorImplementar("RF-HIS-01, agregar una entrada al historial"));
// pacientes.MapGet("/{id}/historial", PorImplementar("RF-HIS-03, ver el historial de un paciente"));

// app.Run();
// return 0;

// static Delegate PorImplementar(string requisito) =>
//     () => Results.Problem(title: "Por implementar", detail: $"{requisito}. Ver docs/srs/SRS.md.", statusCode: 501);

//---------------------- segunda actulalizacion-----------------
// using MedProtect.Api.Data;
// using MedProtect.Api.DTOs;
// using MedProtect.Api.Models;
// using Npgsql;
// using NpgsqlTypes;


// // --------------------------------------------------
// // HEALTHCHECK PARA DOCKER DISTROLESS
// // --------------------------------------------------

// if (args.Contains("--healthcheck"))
// {
//     using var http = new HttpClient
//     {
//         Timeout = TimeSpan.FromSeconds(3)
//     };

//     try
//     {
//         var respuesta = await http.GetAsync(
//             "http://127.0.0.1:8080/api/health"
//         );

//         return respuesta.IsSuccessStatusCode ? 0 : 1;
//     }
//     catch (Exception e) when (
//         e is HttpRequestException or TaskCanceledException
//     )
//     {
//         return 1;
//     }
// }

// // --------------------------------------------------
// // CONFIGURACIÓN DE LA API
// // --------------------------------------------------

// var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(opciones =>
//     opciones.AddServerHeader = false
// );

// var cadena = builder.Configuration
//     .GetConnectionString("MedProtect")
//     ?? throw new InvalidOperationException(
//         "Falta ConnectionStrings__MedProtect."
//     );

// builder.Services.AddSingleton(
//     NpgsqlDataSource.Create(cadena)
// );

// // Registrar repositorios
// builder.Services.AddScoped<PacienteRepository>();
// builder.Services.AddScoped<HistorialRepository>();

// builder.Services.AddProblemDetails();

// var app = builder.Build();

// app.UseExceptionHandler();

// // --------------------------------------------------
// // HEALTH CHECKS
// // --------------------------------------------------

// var api = app.MapGroup("/api");

// api.MapGet("/health", () =>
// {
//     return Results.Ok(new
//     {
//         estado = "viva"
//     });
// });

// api.MapGet("/health/ready", async (
//     NpgsqlDataSource db,
//     ILogger<Program> log,
//     CancellationToken ct) =>
// {
//     try
//     {
//         await using var comando = db.CreateCommand("SELECT 1");

//         await comando.ExecuteScalarAsync(ct);

//         return Results.Ok(new
//         {
//             estado = "lista"
//         });
//     }
//     catch (Exception e) when (
//         e is NpgsqlException or TimeoutException
//     )
//     {
//         log.LogWarning(e, "La base de datos no responde");

//         return Results.Problem(
//             title: "La base de datos no responde",
//             statusCode: 503
//         );
//     }
// });

// // --------------------------------------------------
// // PACIENTES
// // --------------------------------------------------

// var pacientes = api.MapGroup("/pacientes");

// // POST: Registrar paciente

// pacientes.MapPost("/", async (
//     PacienteCrear request,
//     NpgsqlDataSource db,
//     CancellationToken ct) =>
// {
//     if (string.IsNullOrWhiteSpace(request.NombreCompleto) ||
//         string.IsNullOrWhiteSpace(request.CreadoPor))
//     {
//         return Results.BadRequest(new
//         {
//             error = "Nombre completo y creado_por son obligatorios."
//         });
//     }

//     const string sql = """
//         INSERT INTO paciente (
//             nombre_completo,
//             sexo,
//             fecha_nacimiento,
//             telefono,
//             direccion,
//             tipo_sangre,
//             alergias,
//             creado_por
//         )
//         VALUES (
//             @nombre,
//             @sexo,
//             @fecha,
//             @telefono,
//             @direccion,
//             @sangre,
//             @alergias,
//             @creado_por
//         )
//         RETURNING
//             id,
//             nombre_completo,
//             sexo,
//             fecha_nacimiento,
//             telefono,
//             direccion,
//             tipo_sangre,
//             alergias,
//             creado_utc,
//             creado_por;
//         """;

//     await using var comando = db.CreateCommand(sql);

//     comando.Parameters.AddWithValue(
//         "nombre", NpgsqlDbType.Text,
//         request.NombreCompleto.Trim()
//     );

//     comando.Parameters.AddWithValue(
//         "sexo", NpgsqlDbType.Text,
//         (object?)request.Sexo ?? DBNull.Value
//     );

//     comando.Parameters.AddWithValue(
//         "fecha", NpgsqlDbType.Date,
//         (object?)request.FechaNacimiento ?? DBNull.Value
//     );

//     comando.Parameters.AddWithValue(
//         "telefono", NpgsqlDbType.Text,
//         (object?)request.Telefono ?? DBNull.Value
//     );

//     comando.Parameters.AddWithValue(
//         "direccion", NpgsqlDbType.Text,
//         (object?)request.Direccion ?? DBNull.Value
//     );

//     comando.Parameters.AddWithValue(
//         "sangre", NpgsqlDbType.Text,
//         (object?)request.TipoSangre ?? DBNull.Value
//     );

//     comando.Parameters.AddWithValue(
//         "alergias", NpgsqlDbType.Text,
//         (object?)request.Alergias ?? DBNull.Value
//     );

//     comando.Parameters.AddWithValue(
//         "creado_por", NpgsqlDbType.Text,
//         request.CreadoPor.Trim()
//     );

//     await using var reader = await comando.ExecuteReaderAsync(ct);

//     await reader.ReadAsync(ct);

//     var paciente = new PacienteRespuesta(
//         reader.GetGuid(0),
//         reader.GetString(1),
//         reader.IsDBNull(2) ? null : reader.GetString(2),
//         reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3),
//         reader.IsDBNull(4) ? null : reader.GetString(4),
//         reader.IsDBNull(5) ? null : reader.GetString(5),
//         reader.IsDBNull(6) ? null : reader.GetString(6),
//         reader.IsDBNull(7) ? null : reader.GetString(7),
//         reader.GetFieldValue<DateTime>(8),
//         reader.GetString(9)
//     );

//     return Results.Created(
//         $"/api/pacientes/{paciente.Id}",
//         paciente
//     );
// });

// // GET: Consultar paciente

// pacientes.MapGet("/{id:guid}", async (
//     Guid id,
//     NpgsqlDataSource db,
//     CancellationToken ct) =>
// {
//     const string sql = """
//         SELECT
//             id,
//             nombre_completo,
//             sexo,
//             fecha_nacimiento,
//             telefono,
//             direccion,
//             tipo_sangre,
//             alergias,
//             creado_utc,
//             creado_por
//         FROM paciente
//         WHERE id = @id;
//         """;

//     await using var comando = db.CreateCommand(sql);

//     comando.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, id);

//     await using var reader = await comando.ExecuteReaderAsync(ct);

//     if (!await reader.ReadAsync(ct))
//     {
//         return Results.NotFound(new
//         {
//             error = "Paciente no encontrado."
//         });
//     }

//     var paciente = new PacienteRespuesta(
//         reader.GetGuid(0),
//         reader.GetString(1),
//         reader.IsDBNull(2) ? null : reader.GetString(2),
//         reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3),
//         reader.IsDBNull(4) ? null : reader.GetString(4),
//         reader.IsDBNull(5) ? null : reader.GetString(5),
//         reader.IsDBNull(6) ? null : reader.GetString(6),
//         reader.IsDBNull(7) ? null : reader.GetString(7),
//         reader.GetFieldValue<DateTime>(8),
//         reader.GetString(9)
//     );

//     return Results.Ok(paciente);
// });

// // --------------------------------------------------
// // HISTORIAL CLÍNICO
// // --------------------------------------------------

// // POST: Agregar entrada al historial

// pacientes.MapPost("/{id:guid}/historial", async (
//     Guid id,
//     HistorialCrear request,
//     NpgsqlDataSource db,
//     CancellationToken ct) =>
// {
//     if (string.IsNullOrWhiteSpace(request.Descripcion) ||
//         string.IsNullOrWhiteSpace(request.EscritaPor))
//     {
//         return Results.BadRequest(new
//         {
//             error = "Descripción y escrita_por son obligatorios."
//         });
//     }

//     const string sql = """
//         INSERT INTO entrada_historial (
//             paciente_id,
//             descripcion,
//             escrita_por
//         )
//         VALUES (
//             @paciente_id,
//             @descripcion,
//             @escrita_por
//         )
//         RETURNING
//             id,
//             paciente_id,
//             descripcion,
//             escrita_utc,
//             escrita_por;
//         """;

//     try
//     {
//         await using var comando = db.CreateCommand(sql);

//         comando.Parameters.AddWithValue(
//             "paciente_id", NpgsqlDbType.Uuid, id
//         );

//         comando.Parameters.AddWithValue(
//             "descripcion", NpgsqlDbType.Text,
//             request.Descripcion.Trim()
//         );

//         comando.Parameters.AddWithValue(
//             "escrita_por", NpgsqlDbType.Text,
//             request.EscritaPor.Trim()
//         );

//         await using var reader = await comando.ExecuteReaderAsync(ct);

//         await reader.ReadAsync(ct);

//         var entrada = new HistorialRespuesta(
//             reader.GetGuid(0),
//             reader.GetGuid(1),
//             reader.GetString(2),
//             reader.GetFieldValue<DateTime>(3),
//             reader.GetString(4)
//         );

//         return Results.Created(
//             $"/api/pacientes/{id}/historial",
//             entrada
//         );
//     }
//     catch (PostgresException e) when (
//         e.SqlState == "23503"
//     )
//     {
//         return Results.NotFound(new
//         {
//             error = "El paciente no existe."
//         });
//     }
// });

// // GET: Consultar historial de un paciente

// pacientes.MapGet("/{id:guid}/historial", async (
//     Guid id,
//     NpgsqlDataSource db,
//     CancellationToken ct) =>
// {
//     const string sql = """
//         SELECT EXISTS (
//             SELECT 1
//             FROM paciente
//             WHERE id = @id
//         );
//         """;

//     await using var verificar = db.CreateCommand(sql);

//     verificar.Parameters.AddWithValue(
//         "id", NpgsqlDbType.Uuid, id
//     );

//     var existe = (bool)(await verificar.ExecuteScalarAsync(ct))!;

//     if (!existe)
//     {
//         return Results.NotFound(new
//         {
//             error = "Paciente no encontrado."
//         });
//     }

//     const string consulta = """
//         SELECT
//             id,
//             paciente_id,
//             descripcion,
//             escrita_utc,
//             escrita_por
//         FROM entrada_historial
//         WHERE paciente_id = @id
//         ORDER BY escrita_utc DESC;
//         """;

//     await using var comando = db.CreateCommand(consulta);

//     comando.Parameters.AddWithValue(
//         "id", NpgsqlDbType.Uuid, id
//     );

//     await using var reader = await comando.ExecuteReaderAsync(ct);

//     var entradas = new List<HistorialRespuesta>();

//     while (await reader.ReadAsync(ct))
//     {
//         entradas.Add(new HistorialRespuesta(
//             reader.GetGuid(0),
//             reader.GetGuid(1),
//             reader.GetString(2),
//             reader.GetFieldValue<DateTime>(3),
//             reader.GetString(4)
//         ));
//     }

//     return Results.Ok(entradas);
// });

// // --------------------------------------------------
// // INICIAR API
// // --------------------------------------------------

// app.Run();
// return 0;


// // --------------------------------------------------
// // DTOs Y MODELOS DE RESPUESTA
// // --------------------------------------------------

// record PacienteCrear(
//     string NombreCompleto,
//     string? Sexo,
//     DateOnly? FechaNacimiento,
//     string? Telefono,
//     string? Direccion,
//     string? TipoSangre,
//     string? Alergias,
//     string CreadoPor
// );

// record PacienteRespuesta(
//     Guid Id,
//     string NombreCompleto,
//     string? Sexo,
//     DateOnly? FechaNacimiento,
//     string? Telefono,
//     string? Direccion,
//     string? TipoSangre,
//     string? Alergias,
//     DateTime CreadoUtc,
//     string CreadoPor
// );

// record HistorialCrear(
//     string Descripcion,
//     string EscritaPor
// );

// record HistorialRespuesta(
//     Guid Id,
//     Guid PacienteId,
//     string Descripcion,
//     DateTime EscritaUtc,
//     string EscritaPor
// );

// -------------------------------------------------- tercera


using MedProtect.Api.Data;
using MedProtect.Api.DTOs;
using Npgsql;

// Healthcheck interno para Docker distroless
if (args.Contains("--healthcheck"))
{
    using var http = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    try
    {
        var respuesta = await http.GetAsync(
            "http://127.0.0.1:8080/api/health"
        );

        Environment.ExitCode = respuesta.IsSuccessStatusCode ? 0 : 1;
        return;
    }
    catch (Exception e) when (
        e is HttpRequestException or TaskCanceledException
    )
    {
        Environment.ExitCode = 1;
        return;
    }
}

var builder = WebApplication.CreateBuilder(args);

// Seguridad: ocultar cabecera Server
builder.WebHost.ConfigureKestrel(
    opciones => opciones.AddServerHeader = false
);

// Conexión PostgreSQL desde variables de entorno
var cadena = builder.Configuration
    .GetConnectionString("MedProtect")
    ?? throw new InvalidOperationException(
        "Falta ConnectionStrings__MedProtect."
    );

builder.Services.AddSingleton(
    NpgsqlDataSource.Create(cadena)
);

// Registrar repositorios
builder.Services.AddScoped<PacienteRepository>();
builder.Services.AddScoped<HistorialRepository>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

var api = app.MapGroup("/api");

// -----------------------------------------
// HEALTHCHECKS
// -----------------------------------------

api.MapGet("/health", () =>
    Results.Ok(new { estado = "viva" })
);

api.MapGet("/health/ready", async (
    NpgsqlDataSource db,
    ILogger<Program> log,
    CancellationToken ct) =>
{
    try
    {
        await using var comando =
            db.CreateCommand("SELECT 1");

        await comando.ExecuteScalarAsync(ct);

        return Results.Ok(new { estado = "lista" });
    }
    catch (Exception e) when (
        e is NpgsqlException or TimeoutException
    )
    {
        log.LogWarning(e, "La base de datos no responde");

        return Results.Problem(
            title: "La base de datos no responde",
            statusCode: 503
        );
    }
});

// -----------------------------------------
// PACIENTES
// -----------------------------------------

var pacientes = api.MapGroup("/pacientes");

// RF-PAC-01: Alta de paciente
pacientes.MapPost("/", async (
    PacienteCrear request,
    PacienteRepository repositorio,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.NombreCompleto) ||
        string.IsNullOrWhiteSpace(request.CreadoPor))
    {
        return Results.BadRequest(new
        {
            error = "Nombre completo y creadoPor son obligatorios."
        });
    }

    var paciente = await repositorio.CrearAsync(
        request, ct
    );

    return Results.Created(
        $"/api/pacientes/{paciente.Id}",
        paciente
    );
});

// RF-PAC-04: Consultar paciente
pacientes.MapGet("/{id:guid}", async (
    Guid id,
    PacienteRepository repositorio,
    CancellationToken ct) =>
{
    var paciente = await repositorio.ObtenerPorIdAsync(
        id, ct
    );

    if (paciente is null)
    {
        return Results.NotFound(new
        {
            error = "Paciente no encontrado."
        });
    }

    return Results.Ok(paciente);
});

// -----------------------------------------
// HISTORIAL CLÍNICO
// -----------------------------------------

// RF-HIS-01: Registrar entrada clínica
pacientes.MapPost("/{id:guid}/historial", async (
    Guid id,
    HistorialCrear request,
    HistorialRepository repositorio,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Descripcion) ||
        string.IsNullOrWhiteSpace(request.EscritaPor))
    {
        return Results.BadRequest(new
        {
            error = "Descripción y escritaPor son obligatorios."
        });
    }

    var entrada = await repositorio.CrearAsync(
        id, request, ct
    );

    if (entrada is null)
    {
        return Results.NotFound(new
        {
            error = "El paciente no existe."
        });
    }

    return Results.Created(
        $"/api/pacientes/{id}/historial",
        entrada
    );
});

// RF-HIS-03: Consultar historial
pacientes.MapGet("/{id:guid}/historial", async (
    Guid id,
    PacienteRepository pacientesRepo,
    HistorialRepository historialRepo,
    CancellationToken ct) =>
{
    var paciente = await pacientesRepo.ObtenerPorIdAsync(
        id, ct
    );

    if (paciente is null)
    {
        return Results.NotFound(new
        {
            error = "Paciente no encontrado."
        });
    }

    var historial =
        await historialRepo.ObtenerPorPacienteAsync(
            id, ct
        );

    return Results.Ok(historial);
});

// Iniciar aplicación
app.Run();
