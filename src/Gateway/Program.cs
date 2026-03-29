using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using MMLib.Ocelot.Provider.AppConfiguration;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

#region Ocelot
builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = "OcelotConfiguration";
});
builder.Services.AddOcelot(builder.Configuration).AddAppConfiguration();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
#endregion

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:5002";
        options.Audience = "api";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "ExampleIssuer",
            ValidateAudience = true,
            ValidAudience = "ValidAudience",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("fvh2456477hth44j6wfds98dq9hp8bqh9ubq9gjig3qr0[94vj5"))
        };
    });


//Rate limiter
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>(); // Add this line
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false; // Do not stack blocked requests
    options.HttpStatusCode = 429; // Use HTTP 429 Too Many Requests status code
    options.RealIpHeader = "X-Real-IP"; // Use this header to get the real IP address
    options.ClientIdHeader = "X-ClientId"; // Use this header to identify clients
    options.GeneralRules =
    [
        new RateLimitRule
        {
            Endpoint = "*", //rate limit effective for all API endpoints
            Period = "10s",
            Limit = 2 // 2 requests per 10 seconds
        }
    ];
});


builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

#region Ocelot
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";

});
app.UseOcelot().Wait(); 
#endregion

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseIpRateLimiting();

app.Run();