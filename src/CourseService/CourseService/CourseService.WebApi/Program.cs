using CourseService.Application;
using CourseService.Infrastructure;
using CourseService.WebApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

app.Run();
