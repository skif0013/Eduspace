using ContentService.Application.Contracts;
using ContentService.Domain.Models;
using ContentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;
    
    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid Id) => await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == Id);
    
    public async Task AddAsync(Group group) => await _context.Groups.AddAsync(group);
    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    
    public async Task DeleteAsync(Group group)
    {
        _context.Groups.Remove(group);
    }
    
    public async Task<List<Group>> GetAllForUserAsync(Guid userId)
    {
        return await _context.Groups
            .Include(g => g.Members)
            .Where(g => g.Members.Any(m => m.UserId == userId))
            .ToListAsync();
    }
}