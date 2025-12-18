using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Domain.Models;

namespace ContentService.Application.Contracts;

public interface IGroupMemberService
{
    Task<GroupMember> AddMemberToGroupAsync(AddGroupMemberRequestDTO requestDto ,Guid groupId, Guid userId);
    
    Task<GroupMember> RemoveMemberFromGroupAsync(Guid groupId, Guid userId);
    
    Task<GroupMember> GetGroupMemberAsync(Guid groupId, Guid userId);
    
}
