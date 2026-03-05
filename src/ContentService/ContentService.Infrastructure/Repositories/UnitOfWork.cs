using ContentService.Application.Contracts.Repositories;
using ContentService.Infrastructure.Data;

namespace ContentService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}