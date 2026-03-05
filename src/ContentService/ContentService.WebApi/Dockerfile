FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ContentService.WebApi/ContentService.WebApi.csproj ContentService.WebApi/
COPY ContentService.Application/ContentService.Application.csproj ContentService.Application/
COPY ContentService.Domain/ContentService.Domain.csproj ContentService.Domain/
COPY ContentService.Infrastructure/ContentService.Infrastructure.csproj ContentService.Infrastructure/

RUN dotnet restore ContentService.WebApi/ContentService.WebApi.csproj


COPY . .

WORKDIR /src/ContentService.WebApi


RUN dotnet build ContentService.WebApi.csproj -c Release -o /app/build
RUN dotnet publish ContentService.WebApi.csproj -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContentService.WebApi.dll"]
