FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base9
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base8
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-auth
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthService/AuthService.API/AuthService.API.csproj", "AuthService/AuthService.API/"]
COPY ["AuthService/AuthService.Application/AuthService.Application.csproj", "AuthService/AuthService.Application/"]
COPY ["AuthService/AuthService.Domain/AuthService.Domain.csproj", "AuthService/AuthService.Domain/"]
COPY ["AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "AuthService/AuthService.Infrastructure/"]
RUN dotnet restore "AuthService/AuthService.API/AuthService.API.csproj"
COPY AuthService/ AuthService/
WORKDIR /src/AuthService/AuthService.API
RUN dotnet publish "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-auth
COPY --from=build-auth /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-user
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserService/UserService.WebApi/UserService.WebApi.csproj", "UserService/UserService.WebApi/"]
COPY ["UserService/UserService.Application/UserService.Application.csproj", "UserService/UserService.Application/"]
COPY ["UserService/UserService.Domain/UserService.Domain.csproj", "UserService/UserService.Domain/"]
COPY ["UserService/UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService/UserService.Infrastructure/"]
RUN dotnet restore "UserService/UserService.WebApi/UserService.WebApi.csproj"
COPY UserService/ UserService/
WORKDIR /src/UserService/UserService.WebApi
RUN dotnet publish "UserService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-user
COPY --from=build-user /app/publish .
ENTRYPOINT ["dotnet", "UserService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-course
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CourseService/CourseService.WebApi/CourseService.WebApi.csproj", "CourseService/CourseService.WebApi/"]
COPY ["CourseService/CourseService.Application/CourseService.Application.csproj", "CourseService/CourseService.Application/"]
COPY ["CourseService/CourseService.Domain/CourseService.Domain.csproj", "CourseService/CourseService.Domain/"]
COPY ["CourseService/CourseService.Infrastructure/CourseService.Infrastructure.csproj", "CourseService/CourseService.Infrastructure/"]
RUN dotnet restore "CourseService/CourseService.WebApi/CourseService.WebApi.csproj"
COPY CourseService/ CourseService/
WORKDIR /src/CourseService/CourseService.WebApi
RUN dotnet publish "CourseService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-course
COPY --from=build-course /app/publish .
ENTRYPOINT ["dotnet", "CourseService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-notification
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NotificationService/NotificationService.WebApi/NotificationService.WebApi.csproj", "NotificationService/NotificationService.WebApi/"]
COPY ["NotificationService/NotificationService.Application/NotificationService.Application.csproj", "NotificationService/NotificationService.Application/"]
COPY ["NotificationService/NotificationService.Domain/NotificationService.Domain.csproj", "NotificationService/NotificationService.Domain/"]
COPY ["NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "NotificationService/NotificationService.Infrastructure/"]
RUN dotnet restore "NotificationService/NotificationService.WebApi/NotificationService.WebApi.csproj"
COPY NotificationService/ NotificationService/
WORKDIR /src/NotificationService/NotificationService.WebApi
RUN dotnet publish "NotificationService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base9 AS final-notification
COPY --from=build-notification /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-quiz
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QuizService/QuizService.WebApi/QuizService.WebApi.csproj", "QuizService/QuizService.WebApi/"]
COPY ["QuizService/QuizService.Application/QuizService.Application.csproj", "QuizService/QuizService.Application/"]
COPY ["QuizService/QuizService.Domain/QuizService.Domain.csproj", "QuizService/QuizService.Domain/"]
COPY ["QuizService/QuizService.Infrastructure/QuizService.Infrastructure.csproj", "QuizService/QuizService.Infrastructure/"]
RUN dotnet restore "QuizService/QuizService.WebApi/QuizService.WebApi.csproj"
COPY QuizService/ QuizService/
WORKDIR /src/QuizService/QuizService.WebApi
RUN dotnet publish "QuizService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base8 AS final-quiz
COPY --from=build-quiz /app/publish .
ENTRYPOINT ["dotnet", "QuizService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-content
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ContentService/ContentService.WebApi/ContentService.WebApi.csproj", "ContentService/ContentService.WebApi/"]
COPY ["ContentService/ContentService.Application/ContentService.Application.csproj", "ContentService/ContentService.Application/"]
COPY ["ContentService/ContentService.Domain/ContentService.Domain.csproj", "ContentService/ContentService.Domain/"]
COPY ["ContentService/ContentService.Infrastructure/ContentService.Infrastructure.csproj", "ContentService/ContentService.Infrastructure/"]
RUN dotnet restore "ContentService/ContentService.WebApi/ContentService.WebApi.csproj"
COPY ContentService/ ContentService/
WORKDIR /src/ContentService/ContentService.WebApi
RUN dotnet publish "ContentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-content
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build-content /app/publish .
ENTRYPOINT ["dotnet", "ContentService.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-gateway
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OcelotGeatwey/OcelotGeatwey/OcelotGeatwey.csproj", "OcelotGeatwey/OcelotGeatwey/"]
RUN dotnet restore "OcelotGeatwey/OcelotGeatwey/OcelotGeatwey.csproj"
COPY OcelotGeatwey/ OcelotGeatwey/
WORKDIR /src/OcelotGeatwey/OcelotGeatwey
RUN dotnet publish "OcelotGeatwey.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base8 AS final-gateway
COPY --from=build-gateway /app/publish .
ENTRYPOINT ["dotnet", "OcelotGeatwey.dll"]
