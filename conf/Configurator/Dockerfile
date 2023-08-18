FROM mcr.microsoft.com/dotnet/aspnet:6.0.21-bullseye-slim-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.413-bullseye-slim-amd64 AS build
WORKDIR /src
COPY ["Configurator/Configurator.csproj", "Configurator/"]
RUN dotnet restore "Configurator/Configurator.csproj"
COPY . .
WORKDIR "/src/Configurator"
RUN dotnet build "Configurator.csproj" -r linux-arm64 -c Release -o /app/build --self-contained

FROM build AS publish
RUN dotnet publish "Configurator.csproj" -r linux-arm64 -c Release -o /app/publish --self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configurator.dll"]
