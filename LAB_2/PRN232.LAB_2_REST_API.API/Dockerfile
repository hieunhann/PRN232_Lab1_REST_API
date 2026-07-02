# LAB 2 - Advanced REST API & Security
# Multi-stage Docker build cho PRN232.LAB_2_REST_API

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files trước để tận dụng Docker layer caching
COPY ["PRN232.LAB_2_REST_API.API/PRN232.LAB_2_REST_API.API.csproj", "PRN232.LAB_2_REST_API.API/"]
COPY ["PRN232.LAB_2_REST_API.Services/PRN232.LAB_2_REST_API.Services.csproj", "PRN232.LAB_2_REST_API.Services/"]
COPY ["PRN232.LAB_2_REST_API.Repositories/PRN232.LAB_2_REST_API.Repositories.csproj", "PRN232.LAB_2_REST_API.Repositories/"]

# Restore NuGet packages
RUN dotnet restore "./PRN232.LAB_2_REST_API.API/PRN232.LAB_2_REST_API.API.csproj"

# Copy toàn bộ source code
COPY . .

# Build
WORKDIR "/src/PRN232.LAB_2_REST_API.API"
RUN dotnet build "./PRN232.LAB_2_REST_API.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PRN232.LAB_2_REST_API.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LAB_2_REST_API.API.dll"]
