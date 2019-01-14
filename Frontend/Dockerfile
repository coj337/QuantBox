FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY Frontend/Frontend.csproj Frontend/
COPY Services/Sentiment/Sentiment.Infrastructure/Sentiment.Infrastructure.csproj Services/Sentiment/Sentiment.Infrastructure/
COPY Services/Sentiment/Sentiment.Domain/Sentiment.Domain.csproj Services/Sentiment/Sentiment.Domain/
COPY Services/Arbitrage/Arbitrage.Infrastructure/Arbitrage.Infrastructure.csproj Services/Arbitrage/Arbitrage.Infrastructure/
COPY Services/Arbitrage/Arbitrage.Domain/Arbitrage.Domain.csproj Services/Arbitrage/Arbitrage.Domain/
COPY Services/Market/Market.Infrastructure/Market.Infrastructure.csproj Services/Market/Market.Infrastructure/
COPY Services/Market/Market.Domain/Market.Domain.csproj Services/Market/Market.Domain/
RUN dotnet restore Frontend/Frontend.csproj
COPY . .
WORKDIR /src/Frontend
RUN dotnet build Frontend.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Frontend.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Frontend.dll"]
