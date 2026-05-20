# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PRN232.LAB_1_REST_API.API/PRN232.LAB_1_REST_API.API.csproj", "PRN232.LAB_1_REST_API.API/"]
COPY ["PRN232.LAB_1_REST_API.Services/PRN232.LAB_1_REST_API.Services.csproj", "PRN232.LAB_1_REST_API.Services/"]
COPY ["PRN232.LAB_1_REST_API.Repositories/PRN232.LAB_1_REST_API.Repositories.csproj", "PRN232.LAB_1_REST_API.Repositories/"]
RUN dotnet restore "./PRN232.LAB_1_REST_API.API/PRN232.LAB_1_REST_API.API.csproj"
COPY . .
WORKDIR "/src/PRN232.LAB_1_REST_API.API"
RUN dotnet build "./PRN232.LAB_1_REST_API.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PRN232.LAB_1_REST_API.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LAB_1_REST_API.API.dll"]
