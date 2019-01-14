FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY Services/Arbitrage/Arbitrage.API/Arbitrage.API.csproj Services/Arbitrage/Arbitrage.API/
RUN dotnet restore Services/Arbitrage/Arbitrage.API/Arbitrage.API.csproj
COPY . .
WORKDIR /src/Services/Arbitrage/Arbitrage.API
RUN dotnet build Arbitrage.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Arbitrage.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Arbitrage.API.dll"]
