using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using IdentityService.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IdentityService.IntegrationTests.Base;

public static class IdentityScenariosBase
{
    // Общая конфигурация JSON для сериализации/десериализации
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    // Возвращает настроенную фабрику сервера, которую тесты могут использовать для CreateClient()
    public static WebApplicationFactory<Program> CreateServer()
    {
        // Use our CustomWebApplicationFactory configured to use in-memory Sqlite by default
        var factory = new CustomWebApplicationFactory<Program>("UseSqliteInMemory")
            .WithWebHostBuilder(builder =>
            {
                // Гарантируем, что конфигурация берётся из текущей папки теста
                builder.UseContentRoot(Directory.GetCurrentDirectory());
                builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.Sources.Clear();
                    configurationBuilder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                });
            });

        // CustomWebApplicationFactory already applies migrations during ConfigureServices
        return factory;
    }

    public static async Task<T?> GetRequestContent<T>(HttpResponseMessage httpResponseMessage)
    {
        await using var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }

    public static StringContent BuildRequestContent<T>(T content)
    {
        string serialized = JsonSerializer.Serialize(content, JsonOptions);
        return new StringContent(serialized, Encoding.UTF8, "application/json");
    }
}