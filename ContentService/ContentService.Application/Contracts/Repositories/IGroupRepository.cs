using ContentService.Domain.Models;

namespace ContentService.Application.Contracts;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid Id);
    Task AddAsync(Group group);
    Task DeleteAsync(Group group);
    Task<List<Group>> GetAllForUserAsync(Guid userId);
}