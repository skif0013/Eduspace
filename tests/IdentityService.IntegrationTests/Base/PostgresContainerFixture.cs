using Xunit;

namespace IdentityService.IntegrationTests.Base
{
    public class PostgresContainerFixture : IAsyncLifetime
    {
        // Legacy fixture kept for backward compatibility.
        // Integration tests now use in-memory Sqlite via CustomWebApplicationFactory.
        public string ConnectionString => "UseSqliteInMemory";

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}