FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QuizService.WebApi/QuizService.WebApi.csproj", "QuizService.WebApi/"]
COPY ["QuizService.Application/QuizService.Application.csproj", "QuizService.Application/"]
COPY ["QuizService.Domain/QuizService.Domain.csproj", "QuizService.Domain/"]
COPY ["QuizService.Infrastructure/QuizService.Infrastructure.csproj", "QuizService.Infrastructure/"]
RUN dotnet restore "QuizService.WebApi/QuizService.WebApi.csproj"
COPY . .
WORKDIR "/src/QuizService.WebApi"
RUN dotnet build "./QuizService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QuizService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuizService.WebApi.dll"]
