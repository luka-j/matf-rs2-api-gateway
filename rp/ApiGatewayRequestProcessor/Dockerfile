FROM mcr.microsoft.com/dotnet/aspnet:7.0.2-bullseye-slim-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0.102-bullseye-slim-amd64 AS build
WORKDIR /src
COPY ["ApiGatewayRequestProcessor/ApiGatewayRequestProcessor.csproj", "ApiGatewayRequestProcessor/"]
RUN dotnet restore "ApiGatewayRequestProcessor/ApiGatewayRequestProcessor.csproj"
COPY . .
WORKDIR "/src/ApiGatewayRequestProcessor"
RUN dotnet build "ApiGatewayRequestProcessor.csproj" -r linux-arm64 -c Release -o /app/build --self-contained

FROM build AS publish
RUN dotnet publish "ApiGatewayRequestProcessor.csproj" -r linux-arm64 -c Release -o /app/publish --self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGatewayRequestProcessor.dll"]
