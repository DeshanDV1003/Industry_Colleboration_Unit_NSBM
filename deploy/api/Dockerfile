# NSBM ICU API — build from the repository root:
#   docker build -f deploy/api/Dockerfile -t nsbm-icu-api .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first, so the package layer is cached until a project file changes.
COPY Directory.Build.props NSBM.ICU.slnx ./
COPY src/NSBM.ICU.Domain/NSBM.ICU.Domain.csproj src/NSBM.ICU.Domain/
COPY src/NSBM.ICU.Application/NSBM.ICU.Application.csproj src/NSBM.ICU.Application/
COPY src/NSBM.ICU.Infrastructure/NSBM.ICU.Infrastructure.csproj src/NSBM.ICU.Infrastructure/
COPY src/NSBM.ICU.API/NSBM.ICU.API.csproj src/NSBM.ICU.API/
RUN dotnet restore src/NSBM.ICU.API/NSBM.ICU.API.csproj

COPY src/ src/
RUN dotnet publish src/NSBM.ICU.API/NSBM.ICU.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Runs as the image's unprivileged "app" user on port 8080. TLS is terminated by the proxy in front.
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true
USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "NSBM.ICU.API.dll"]
