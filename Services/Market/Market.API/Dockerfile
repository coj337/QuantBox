FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY Services/Market/Market.API/Market.API.csproj Services/Market/Market.API/
COPY Services/Market/Market.Infrastructure/Market.Infrastructure.csproj Services/Market/Market.Infrastructure/
COPY Services/Market/Market.Domain/Market.Domain.csproj Services/Market/Market.Domain/
RUN dotnet restore Services/Market/Market.API/Market.API.csproj
COPY . .
WORKDIR /src/Services/Market/Market.API
RUN dotnet build Market.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Market.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Market.API.dll"]
