FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/StatisticsService/StatisticsService.WebApi/StatisticsService.WebApi.csproj", "src/StatisticsService/StatisticsService.WebApi/"]
RUN dotnet restore "src/StatisticsService/StatisticsService.WebApi/StatisticsService.WebApi.csproj"
COPY . .
WORKDIR "/src/src/StatisticsService/StatisticsService.WebApi"
RUN dotnet build "./StatisticsService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StatisticsService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatisticsService.WebApi.dll"]
