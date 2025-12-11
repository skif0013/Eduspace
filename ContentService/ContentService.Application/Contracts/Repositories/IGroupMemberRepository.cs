using ContentService.Domain.Models;

namespace ContentService.Application.Contracts;

public interface IGroupMemberRepository
{
    Task<GroupMember> GetAsync(Guid userId, Guid groupId);
    Task AddAsync(GroupMember groupMember);
    Task SaveChangesAsync();
    
    Task<GroupMember> DeleteAsync(Guid userId, Guid groupId);
}