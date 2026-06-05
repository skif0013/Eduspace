using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using IdentityService.Infrastructure.Database;

namespace IdentityService.IntegrationTests.Base
{
    // Test WebApplicationFactory that swaps ApplicationDbContext to Sqlite in-memory or Postgres.
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _connectionString;

        public CustomWebApplicationFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove all existing ApplicationDbContext registrations/options so only one provider remains.
                services.RemoveAll(typeof(ApplicationDbContext));
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                var optionsConfigurations = services
                    .Where(d => d.ServiceType.IsGenericType
                        && d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>)
                        && d.ServiceType.GenericTypeArguments[0] == typeof(ApplicationDbContext))
                    .ToList();
                foreach (var descriptor in optionsConfigurations)
                {
                    services.Remove(descriptor);
                }

                // Remove hosted services that may rely on external infra (Redis, background jobs)
                var hosted = services.Where(d => d.ServiceType != null && d.ServiceType.FullName == "Microsoft.Extensions.Hosting.IHostedService").ToList();
                foreach (var d in hosted)
                {
                    // remove probable background jobs by name
                    var implName = d.ImplementationType?.Name ?? d.ServiceType.Name;
                    if (implName.Contains("RedisSubscriberService") || implName.Contains("ProcessOutboxMessagesJob"))
                    {
                        services.Remove(d);
                    }
                }

                // Remove Redis / connection multiplexer registrations by name to avoid external connections
                var redisDescriptors = services.Where(d => (d.ServiceType?.FullName != null && d.ServiceType.FullName.Contains("StackExchange.Redis")) || (d.ImplementationType?.FullName != null && d.ImplementationType.FullName.Contains("StackExchange.Redis"))).ToList();
                foreach (var d in redisDescriptors) services.Remove(d);

                // Finally, register ApplicationDbContext. Support two modes:
                // 1) If _connectionString equals the special token "UseSqliteInMemory" or
                //    environment variable TEST_USE_SQLITE=1 is set, use an in-memory SQLite DB
                //    (no Docker/Testcontainers required). This is useful for local dev and CI
                //    where Docker may be unavailable or blocked by AppLocker.
                // 2) Otherwise, assume a Postgres connection string (Testcontainers) and use Npgsql.

                var useSqliteEnv = Environment.GetEnvironmentVariable("TEST_USE_SQLITE");
                var useSqliteToken = string.Equals(_connectionString, "UseSqliteInMemory", StringComparison.OrdinalIgnoreCase);
                if (useSqliteToken || string.Equals(useSqliteEnv, "1"))
                {
                    // Create a shared in-memory Sqlite connection so the database survives across
                    // multiple DbContext instances during the test lifetime.
                    // Use a unique shared in-memory database per factory instance to avoid cross-test collisions.
                    var sqliteConn = new SqliteConnection($"DataSource=file:memdb-{Guid.NewGuid():N}?mode=memory&cache=shared");
                    sqliteConn.Open();
                    // Keep the connection alive for the lifetime of the test host
                    services.AddSingleton<DbConnection>(sqliteConn);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(sqliteConn));
                }
                else
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(_connectionString));
                }

                // Build a temporary provider to apply migrations before the application starts
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    if (useSqliteToken || string.Equals(useSqliteEnv, "1"))
                    {
                        // For in-memory Sqlite in tests, EnsureCreated is more robust than provider-specific migrations.
                        db.Database.EnsureCreated();
                    }
                    else
                    {
                        db.Database.Migrate();
                    }
                }
            });
        }
    }
}

