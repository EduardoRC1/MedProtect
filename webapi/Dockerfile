# syntax=docker/dockerfile:1
# API de MedProtect. Se construye con la carpeta webapi/ como contexto (lo hace compose.yaml).

# Etapa 1: compilar con el SDK completo. Nada de esta etapa llega a la imagen final.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY global.json ./
COPY MedProtect.Api/MedProtect.Api.csproj MedProtect.Api/
RUN dotnet restore MedProtect.Api/MedProtect.Api.csproj
COPY MedProtect.Api/ MedProtect.Api/
RUN dotnet publish MedProtect.Api/MedProtect.Api.csproj -c Release -o /app --no-restore

# Etapa 2: distroless. La imagen "chiseled" de Microsoft no trae shell, ni gestor de paquetes, ni root:
# corre con el usuario app (uid 1654). Si alguien entra a la API, no tiene con qué moverse.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0
USER 1654
EXPOSE 8080
# Sin shell no hay curl: la API se revisa a sí misma (ver --healthcheck en Program.cs).
HEALTHCHECK --interval=15s --timeout=5s --start-period=10s --retries=4 \
    CMD ["dotnet", "MedProtect.Api.dll", "--healthcheck"]
ENTRYPOINT ["dotnet", "MedProtect.Api.dll"]
