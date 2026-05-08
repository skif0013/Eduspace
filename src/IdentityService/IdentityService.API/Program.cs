using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityService.API.Middleware;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Database;
using IdentityService.Infrastructure.Database.InitialData;
using IdentityService.Infrastructure.Identity;
using IdentityService.Infrastructure.Redis;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Services;
using DotNetEnv;
using IdentityService.Application.Common.Models;
//using IdentityService.Infrastructure.BackgroundJobs;
using IdentityService.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
    Env.Load(envPath);
}

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity Service API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

/*builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5010);
    options.ListenAnyIP(5011);
});*/

#region config jwt
var validIssuer = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidIssuer");
var validAudience = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidAudience");
var symmetricSecurityKey = builder.Configuration.GetValue<string>("JwtTokenSettings:SymmetricSecurityKey");

builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine($"Auth Failed. Exception: {context.Exception.Message}");
                // Если проблема в подписи, тут будет написано "Signature validation failed"
                Console.WriteLine("-------------------------------------------");
                return Task.CompletedTask;
            }
        };
    });
#endregion

// ПОСМОТРИ В КОНСОЛЬ ПРИ ЗАПУСКЕ:
Console.WriteLine($"[CONFIG] Issuer: {validIssuer}");
Console.WriteLine($"[CONFIG] Audience: {validAudience}");
Console.WriteLine($"[CONFIG] Key Length: {symmetricSecurityKey?.Length ?? 0}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));




var redisEndPoint = builder.Configuration["RedisEndPoint"];
var redisUser = builder.Configuration["RedisUser"];
var redisPassword = builder.Configuration["RedisPassword"];


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = new StackExchange.Redis.ConfigurationOptions
    {
        EndPoints = { redisEndPoint },
        User = redisUser,
        Password = redisPassword,
        AbortOnConnectFail = false
    };
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSingleton<IRedisMessageBroker, RedisMessageBroker>();

builder.Services.AddScoped<UserContext>();

builder.Services.AddIdentity<User, RoleIdentity>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = null;
})
.AddRoles<RoleIdentity>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>(); 

builder.Services.AddSingleton<IMessageHandler, UserUpdatedHandler>();
builder.Services.AddHostedService<RedisSubscriberService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddHostedService<ProcessOutboxMessagesJob>();

var app = builder.Build();

app.UseMiddleware<CustomExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    { 
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService API v1");
        options.RoutePrefix = "swagger"; 
    });
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<UserContextMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<RoleIdentity>>();
    
    await RoleInitData.InitializeAsync(roleManager);
}

app.MapControllers();

app.Run();