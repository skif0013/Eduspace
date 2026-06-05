FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base9
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base8
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-identity
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/IdentityService/IdentityService.API/IdentityService.API.csproj", "src/IdentityService/IdentityService.API/"]
COPY ["src/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "src/IdentityService/IdentityService.Application/"]
COPY ["src/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "src/IdentityService/IdentityService.Domain/"]
COPY ["src/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "src/IdentityService/IdentityService.Infrastructure/"]
RUN dotnet restore "src/IdentityService/IdentityService.API/IdentityService.API.csproj"
COPY src/IdentityService/ src/IdentityService/
WORKDIR /src/src/IdentityService/IdentityService.API
RUN dotnet publish "IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-identity
COPY --from=build-identity /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-course
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CourseService/CourseService.WebApi/CourseService.WebApi.csproj", "src/CourseService/CourseService.WebApi/"]
COPY ["src/CourseService/CourseService.Application/CourseService.Application.csproj", "src/CourseService/CourseService.Application/"]
COPY ["src/CourseService/CourseService.Domain/CourseService.Domain.csproj", "src/CourseService/CourseService.Domain/"]
COPY ["src/CourseService/CourseService.Infrastructure/CourseService.Infrastructure.csproj", "src/CourseService/CourseService.Infrastructure/"]
RUN dotnet restore "src/CourseService/CourseService.WebApi/CourseService.WebApi.csproj"
COPY src/CourseService/ src/CourseService/
WORKDIR /src/src/CourseService/CourseService.WebApi
RUN dotnet publish "CourseService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base8 AS final-course
COPY --from=build-course /app/publish .
ENTRYPOINT ["dotnet", "CourseService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-notification
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NotificationService/NotificationService.WebApi/NotificationService.WebApi.csproj", "src/NotificationService/NotificationService.WebApi/"]
COPY ["src/NotificationService/NotificationService.Application/NotificationService.Application.csproj", "src/NotificationService/NotificationService.Application/"]
COPY ["src/NotificationService/NotificationService.Domain/NotificationService.Domain.csproj", "src/NotificationService/NotificationService.Domain/"]
COPY ["src/NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "src/NotificationService/NotificationService.Infrastructure/"]
COPY ["src/QuizService/QuizService.Application/QuizService.Application.csproj", "src/QuizService/QuizService.Application/"]
COPY ["src/QuizService/QuizService.Domain/QuizService.Domain.csproj", "src/QuizService/QuizService.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Redis/BuildingBlocks.Redis.csproj", "src/BuildingBlocks/BuildingBlocks.Redis/"]
RUN dotnet restore "src/NotificationService/NotificationService.WebApi/NotificationService.WebApi.csproj"
COPY src/NotificationService/ src/NotificationService/
COPY src/QuizService/ src/QuizService/
COPY src/BuildingBlocks/ src/BuildingBlocks/
WORKDIR /src/src/NotificationService/NotificationService.WebApi
RUN dotnet publish "NotificationService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-notification
COPY --from=build-notification /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-quiz
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/QuizService/QuizService.WebApi/QuizService.WebApi.csproj", "src/QuizService/QuizService.WebApi/"]
COPY ["src/QuizService/QuizService.Application/QuizService.Application.csproj", "src/QuizService/QuizService.Application/"]
COPY ["src/QuizService/QuizService.Domain/QuizService.Domain.csproj", "src/QuizService/QuizService.Domain/"]
COPY ["src/QuizService/QuizService.Infrastructure/QuizService.Infrastructure.csproj", "src/QuizService/QuizService.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Redis/BuildingBlocks.Redis.csproj", "src/BuildingBlocks/BuildingBlocks.Redis/"]
RUN dotnet restore "src/QuizService/QuizService.WebApi/QuizService.WebApi.csproj"
COPY src/QuizService/ src/QuizService/
COPY src/BuildingBlocks/ src/BuildingBlocks/
WORKDIR /src/src/QuizService/QuizService.WebApi
RUN dotnet publish "QuizService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base8 AS final-quiz
COPY --from=build-quiz /app/publish .
ENTRYPOINT ["dotnet", "QuizService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-file
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FileService/FileService.WebApi/FileService.WebApi.csproj", "src/FileService/FileService.WebApi/"]
COPY ["src/FileService/FileService.Application/FileService.Application.csproj", "src/FileService/FileService.Application/"]
COPY ["src/FileService/FileService.Domain/FileService.Domain.csproj", "src/FileService/FileService.Domain/"]
COPY ["src/FileService/FileService.Infrastructure/FileService.Infrastructure.csproj", "src/FileService/FileService.Infrastructure/"]
RUN dotnet restore "src/FileService/FileService.WebApi/FileService.WebApi.csproj"
COPY src/FileService/ src/FileService/
WORKDIR /src/src/FileService/FileService.WebApi
RUN dotnet publish "FileService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base8 AS final-file
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build-file /app/publish .
ENTRYPOINT ["dotnet", "FileService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-gateway
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Gateway/OcelotGeatwey.csproj", "src/Gateway/"]
RUN dotnet restore "src/Gateway/OcelotGeatwey.csproj"
COPY src/Gateway/ src/Gateway/
WORKDIR /src/src/Gateway
RUN dotnet publish "OcelotGeatwey.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-gateway
COPY --from=build-gateway /app/publish .
ENTRYPOINT ["dotnet", "OcelotGeatwey.dll"]
