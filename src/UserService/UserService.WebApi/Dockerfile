FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserService.WebApi/UserService.WebApi.csproj", "UserService.WebApi/"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]
COPY ["UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/"]
RUN dotnet restore "UserService.WebApi/UserService.WebApi.csproj"
COPY . .
WORKDIR "/src/UserService.WebApi"
RUN dotnet build "./UserService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.WebApi.dll"]
