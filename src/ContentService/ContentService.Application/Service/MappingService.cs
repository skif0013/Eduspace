using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Domain.Models;

namespace ContentService.Application.Service;

public class MappingService
{
    public GroupResponseDTO ToGroupResponseDTO(Group? group)
    {
        if (group is null)
            return new GroupResponseDTO();

        return new GroupResponseDTO
        {
            Id = group.Id,
            GroupName = group.GroupName ?? string.Empty,
            Description = group.Description ?? string.Empty,
            CreatedBy = group.CreatedBy,
            Email = group.Email ?? string.Empty,
            MembersCount = group.Members?.Count ?? 0,
            Members = group.Members?.Select(ToGroupMemberResponseDTO).ToList() ?? new List<GroupMemberResponseDTO>()
        };
    }

    // Map Domain Group -> UpdateGroupDTO (used to return updated values)
    public UpdateGroupDTO ToUpdateGroupDTO(Group? group)
    {
        if (group is null)
            return new UpdateGroupDTO();

        return new UpdateGroupDTO
        {
            GroupName = group.GroupName,
            Description = group.Description
        };
    }
    
    // Map Domain GroupMember -> GroupMemberResponseDTO
    public GroupMemberResponseDTO ToGroupMemberResponseDTO(GroupMember? member)
    {
        if (member is null)
            return new GroupMemberResponseDTO();

        return new GroupMemberResponseDTO
        {
            UserId = member.UserId,
            Email = member.Email ?? string.Empty,
            UserName = member.UserName ?? string.Empty
        };
    }
    
    public List<GroupResponseDTO> ToGroupResponseList(IEnumerable<Group>? groups)
    {
        if (groups is null)
            return new List<GroupResponseDTO>();

        return groups.Select(ToGroupResponseDTO).ToList();
    }
}