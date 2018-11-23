FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 65451
EXPOSE 44325

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["History.Infrastructure/History.Infrastructure.csproj", "History.Infrastructure/"]
RUN dotnet restore "History.Infrastructure/History.Infrastructure.csproj"
COPY . .
WORKDIR "/src/History.Infrastructure"
RUN dotnet build "History.Infrastructure.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "History.Infrastructure.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "History.Infrastructure.dll"]