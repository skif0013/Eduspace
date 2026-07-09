using BuildingBlocks.Redis;
using IdentityService.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityService.Application.Interfaces; 

namespace IdentityService.Infrastructure.BackgroundJobs;

public class ProcessOutboxMessagesJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;        
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private readonly IEventPublisher _eventPublisher; 

    public ProcessOutboxMessagesJob(
        IServiceProvider serviceProvider,
        ILogger<ProcessOutboxMessagesJob> logger,
        IEventPublisher eventPublisher) 
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope(); 
                
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>(); 
                
                var messages = await outboxRepository.GetUnprocessedMessagesAsync(20);

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        try
                        {
                            await _eventPublisher.PublishRawAsync(message.Type, message.Content);
                            
                            _logger.LogInformation("Sent outbox message {Id} to Redis channel {Channel}", message.Id, message.Type);
                            
                            message.ProcessedOnUtc = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
                            message.Error = ex.Message;
                        }
                    }
                    
                    await unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Outbox worker failed unexpectedly");
            }
            //for production more
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}