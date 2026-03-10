using FileService.Application.Contracts.Repositories;
using FileService.Infrastructure.Data;

namespace FileService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}