FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY Services/Trading/Trading.API/Trading.API.csproj Services/Trading/Trading.API/
COPY BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj BuildingBlocks/EventBus/EventBusRabbitMQ/
COPY BuildingBlocks/EventBus/EventBus/EventBus.csproj BuildingBlocks/EventBus/EventBus/
COPY BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj BuildingBlocks/EventBus/EventBusServiceBus/
COPY BuildingBlocks/ExchangeInfrastructure/ExchangeManager/ExchangeManager.csproj BuildingBlocks/ExchangeInfrastructure/ExchangeManager/
COPY ../CoinjarApiClient/CoinjarApiClient.csproj ../CoinjarApiClient/
COPY BuildingBlocks/ExchangeInfrastructure/CustomImplementations/BtcMarketsClient/BtcMarketsApiClient.csproj BuildingBlocks/ExchangeInfrastructure/CustomImplementations/BtcMarketsClient/
RUN dotnet restore Services/Trading/Trading.API/Trading.API.csproj
COPY . .
WORKDIR /src/Services/Trading/Trading.API
RUN dotnet build Trading.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Trading.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Trading.API.dll"]
