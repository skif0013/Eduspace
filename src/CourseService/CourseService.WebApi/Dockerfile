FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CourseService.WebApi/CourseService.WebApi.csproj", "CourseService.WebApi/"]
COPY ["CourseService.Application/CourseService.Application.csproj", "CourseService.Application/"]
COPY ["CourseService.Domain/CourseService.Domain.csproj", "CourseService.Domain/"]
COPY ["CourseService.Infrastructure/CourseService.Infrastructure.csproj", "CourseService.Infrastructure/"]

RUN dotnet restore "CourseService.WebApi/CourseService.WebApi.csproj"

COPY . .

WORKDIR /src/CourseService.WebApi
RUN dotnet build "CourseService.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CourseService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CourseService.WebApi.dll"]