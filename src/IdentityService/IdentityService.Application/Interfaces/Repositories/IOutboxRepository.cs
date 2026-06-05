using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories;

public interface IOutboxRepository : IRepository<OutboxMessage>
{
    //Task AddAsync(OutboxMessage message);
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize);
}