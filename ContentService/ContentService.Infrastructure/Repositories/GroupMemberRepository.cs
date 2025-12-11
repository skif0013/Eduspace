using ContentService.Application.Contracts;
using ContentService.Domain.Models;
using ContentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Infrastructure.Repositories;

public class GroupMemberRepository : IGroupMemberRepository
{
    private readonly ApplicationDbContext _context;
    
    public GroupMemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<GroupMember> GetAsync(Guid userId, Guid groupId) => 
        await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
    
    public async Task AddAsync(GroupMember groupMember) => await _context.GroupMembers.AddAsync(groupMember);
    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<GroupMember> DeleteAsync(Guid userId, Guid groupId) 
    {
        var member =  await GetAsync(userId, groupId) ?? throw new Exception("Group member not found");
        
        _context.GroupMembers.Remove(member);
        
        return member;  
    }
}