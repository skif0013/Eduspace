using Azure.Storage.Blobs;
using FileService.Application.Contracts.Repositories;
using FileService.Application.Mapper;
using FileService.Application.Service;
using FileService.Infrastructure.Data;
using FileService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5010);
    options.ListenAnyIP(5011);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Service API", Version = "v1" });
});

builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"])
);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("FileService.Infrastructure")));

builder.Services.AddAuthorization();


var redisPassword = builder.Configuration.GetValue<string>("RedisPassword");

Console.WriteLine($"Redis Password: {redisPassword}");


builder.Services.AddAutoMapper(typeof(FileMappingProfile).Assembly);
builder.Services.AddScoped<IBlobService,BlobStorageService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService.Application.Service.FileService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
