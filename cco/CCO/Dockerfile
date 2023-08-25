FROM mcr.microsoft.com/dotnet/aspnet:6.0.21-bullseye-slim-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.413-bullseye-slim-amd64 AS build
WORKDIR /src
COPY ["CCO/CCO.csproj", "CCO/"]
RUN dotnet restore "CCO/CCO.csproj"
COPY . .
WORKDIR "/src/CCO"
RUN dotnet build "CCO.csproj" -r linux-arm64 -c Release -o /app/build --self-contained

FROM build AS publish
RUN dotnet publish "CCO.csproj" -r linux-arm64 -c Release -o /app/publish --self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CCO.dll"]
