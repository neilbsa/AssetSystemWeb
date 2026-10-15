# syntax=docker/dockerfile:1
# Build context: repository root.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, in its own layer, so source edits don't re-download packages.
COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/ src/
RUN dotnet restore src/AssetSystem.Host/AssetSystem.Host.csproj
RUN dotnet publish src/AssetSystem.Host/AssetSystem.Host.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Uploaded documents live here; mount a volume to keep them.
RUN mkdir -p /app/data/files && chown -R $APP_UID /app/data
VOLUME /app/data
ENV ASPNETCORE_URLS=http://+:8080 \
    FileStore__RootPath=/app/data/files
EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["dotnet", "AssetSystem.Host.dll"]
