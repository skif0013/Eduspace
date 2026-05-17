using System.Threading.Tasks;
using FluentAssertions;
using IdentityService.IntegrationTests.Base;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityService.IntegrationTests.Tests
{
    // Пример простого интеграционного теста, проверяющего что приложение может подключиться к in-memory DB
    public class DatabaseTests : IntegrationTestBase
    {
        public DatabaseTests() : base()
        {
        }

        [Fact]
        public async Task Database_CanConnect()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityService.Infrastructure.Database.ApplicationDbContext>();
            var canConnect = await db.Database.CanConnectAsync();
            canConnect.Should().BeTrue();
        }
    }
}

