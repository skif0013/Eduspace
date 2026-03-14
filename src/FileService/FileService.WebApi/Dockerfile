FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY FileService.WebApi/FileService.WebApi.csproj FileService.WebApi/
COPY FileService.Application/FileService.Application.csproj FileService.Application/
COPY FileService.Domain/FileService.Domain.csproj FileService.Domain/
COPY ["FileService.Infrastructure/FileService.Infrastructure.csproj", "FileService.Infrastructure/"]

RUN dotnet restore FileService.WebApi/FileService.WebApi.csproj


COPY . .

WORKDIR /src/FileService.WebApi


RUN dotnet build FileService.WebApi.csproj -c Release -o /app/build
RUN dotnet publish FileService.WebApi.csproj -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileService.WebApi.dll"]
