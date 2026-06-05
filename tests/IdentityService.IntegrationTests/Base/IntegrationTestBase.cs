using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.IntegrationTests.Base
{
    // Базовый класс для интеграционных тестов. Использует in-memory SQLite по умолчанию.
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected CustomWebApplicationFactory<Program> Factory = null!;
        protected HttpClient Client = null!;

        protected IntegrationTestBase()
        {
        }

        public virtual async Task InitializeAsync()
        {
            // Use the special token to instruct factory to use in-memory Sqlite
            Factory = new CustomWebApplicationFactory<Program>("UseSqliteInMemory");
            Client = Factory.CreateClient();

            // Ensure test schema exists
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityService.Infrastructure.Database.ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        public virtual Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}

