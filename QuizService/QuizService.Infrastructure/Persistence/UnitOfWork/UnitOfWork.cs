using QuizService.Application.Repositories;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    
}