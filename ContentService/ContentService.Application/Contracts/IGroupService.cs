using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Domain.Models;

namespace ContentService.Application.Contracts;

public interface IGroupService
{
    Task<Group> CreateGroupAsync(CreatingGroupRequestDTO requestDto,Guid userId,string email);
    Task<Group> DeleteGroupAsync(Guid groupId, Guid userId);
    Task<List<Group>> GetAllGroupsAsync(Guid userId);
    Task<Group> GetGroupAsync(Guid groupId, Guid userId);
    Task<Group> UpdateGroupAsync(UpdateGroupDTO dto, Guid groupId, Guid userId);
}