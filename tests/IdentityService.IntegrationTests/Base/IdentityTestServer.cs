using IdentityService.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.IntegrationTests.Base;

public class IdentityTestServer : TestServer
{
    public IdentityTestServer(IWebHostBuilder builder) : base(builder)
    {
        ApplicationDbContext = Host.Services.GetRequiredService<ApplicationDbContext>();
    }

    public ApplicationDbContext ApplicationDbContext { get; set; }
}