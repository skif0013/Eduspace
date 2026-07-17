using LiveService.WebApi.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllCors", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Разрешает абсолютно любой Origin динамически!
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Критически важно для SignalR (передача токенов/сокетов)
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllCors"); 

app.UseAuthorization();

app.MapControllers();
app.MapHub<LiveChatHub>("/livechat");

app.Run();