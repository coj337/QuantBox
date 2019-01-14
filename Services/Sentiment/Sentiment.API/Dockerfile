FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY Services/Sentiment/Sentiment.API/Sentiment.API.csproj Services/Sentiment/Sentiment.API/
COPY Services/Sentiment/Sentiment.Infrastructure/Sentiment.Infrastructure.csproj Services/Sentiment/Sentiment.Infrastructure/
COPY Services/Sentiment/Sentiment.Domain/Sentiment.Domain.csproj Services/Sentiment/Sentiment.Domain/
RUN dotnet restore Services/Sentiment/Sentiment.API/Sentiment.API.csproj
COPY . .
WORKDIR /src/Services/Sentiment/Sentiment.API
RUN dotnet build Sentiment.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Sentiment.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sentiment.API.dll"]
