FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:3.0-sdk AS build
WORKDIR /src
COPY ["Arbitrage.API/Arbitrage.API.csproj", "Arbitrage.API/"]
RUN dotnet restore "Arbitrage.API/Arbitrage.API.csproj"
COPY . .
WORKDIR "/src/Arbitrage.API"
RUN dotnet build "Arbitrage.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Arbitrage.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Arbitrage.API.dll"]