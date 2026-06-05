using IdentityService.Application.Interfaces;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Infrastructure.Database;
using IdentityService.Infrastructure.Repositories;

namespace IdentityService.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IOutboxRepository OutboxRepository => new OutboxRepository(_context);
    public ITokenRepository TokenRepository => new TokenRepository(_context);

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
    
}