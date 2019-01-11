FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:3.0-sdk AS build
WORKDIR /src
COPY ["Sentiment.API/Sentiment.API.csproj", "Sentiment.API/"]
RUN dotnet restore "Sentiment.API/Sentiment.API.csproj"
COPY . .
WORKDIR "/src/Sentiment.API"
RUN dotnet build "Sentiment.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Sentiment.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sentiment.API.dll"]