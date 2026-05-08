using IdentityService.Application.Interfaces.Repositories;

namespace IdentityService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task Commit();
    
    IOutboxRepository OutboxRepository { get; }
}