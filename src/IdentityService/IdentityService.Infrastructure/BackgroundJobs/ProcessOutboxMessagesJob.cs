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
                using var scope = _serviceProvider.CreateScope(); 
                
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var unitOfWorService = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                
                var messages = await outboxRepository.GetUnprocessedMessagesAsync(20);

                foreach (var message in messages)
                {
                    try
                    {
                        await messageService.SendMessageAsync(message.Type, message.Content);
                        _logger.LogInformation("Sent outbox message {Id} to Redis stream", message.Id);
                        message.ProcessedOnUtc = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
                        message.Error = ex.Message;
                    }
                }
                
                await unitOfWorService.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Outbox worker failed unexpectedly");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}