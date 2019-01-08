FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 65258
EXPOSE 44331

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Historey.API/Historey.API.csproj", "Historey.API/"]
RUN dotnet restore "Historey.API/Historey.API.csproj"
COPY . .
WORKDIR "/src/Historey.API"
RUN dotnet build "Historey.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Historey.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Historey.API.dll"]