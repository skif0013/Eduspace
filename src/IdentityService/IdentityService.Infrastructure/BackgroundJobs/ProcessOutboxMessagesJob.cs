using IdentityService.Application.Interfaces;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.BackgroundJobs;

public class ProcessOutboxMessagesJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;        // TODO почитать про IServiceScopeFactory
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        IServiceProvider serviceProvider,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Создаем scope для получения Scoped сервисов (DB context)
                using var scope = _serviceProvider.CreateScope(); 
                
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var unitOfWorService = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // 1. Получаем пачку сообщений
                var messages = await outboxRepository.GetUnprocessedMessagesAsync(20);

                foreach (var message in messages)
                {
                    try
                    {
                        // 2. Отправляем в RabbitMQ (в поле Type можно хранить имя очереди)
                        // Тут нужна будет логика десериализации или просто отправка строки
                        await messageService.SendMessageAsync(message.Type, message.Content);
                        Console.WriteLine($"Sent outbox message {message.Id} to Redis pub/sub");
                        // 3. Помечаем как успешно обработанное
                        message.ProcessedOnUtc = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
                        //message.Error = ex.Message;
                        Console.WriteLine($"Error processing outbox message {message.Id}: {ex.Message}");
                    }
                }
                
                // 4. Обновляем статус в БД
                await unitOfWorService.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Outbox worker failed unexpectedly");
            }
            
            
            // Ждем 5 секунд перед следующим кругом
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}