using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class OutboxRepository : Repository<OutboxMessage>, IOutboxRepository
{
    
    // private readonly ApplicationDbContext _context;
    //
    // public OutboxRepository(ApplicationDbContext context)
    // {
    //     _context = context;
    // }
    
    public OutboxRepository(ApplicationDbContext context) : base(context) 
    { 
    }
    
    public Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize)
    {
        return _dbSet
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync();
    }
}   