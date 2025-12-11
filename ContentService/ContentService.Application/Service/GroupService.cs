using ContentService.Application.Contracts;
using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Domain.Models;

namespace ContentService.Application.Service;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    
    
    public GroupService(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
        
    }
    
    public async Task<Group> CreateGroupAsync(CreatingGroupRequestDTO requestDto,Guid userId,string email)
    {
        var group = new Group()
        {
            Id = Guid.NewGuid(),
            GroupName = requestDto.GroupName,
            Description = requestDto.Description,
            CreatedBy =  userId,
            Email = email,
            Members = new List<GroupMember>
            {
                new GroupMember { UserId = userId, Email = email, UserName = ""},
            }
        };
      
        await _groupRepository.AddAsync(group);
        await _groupRepository.SaveChangesAsync();

        return group;
    }
    
    public async Task<Group> DeleteGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        
        if (group == null)
        {
            throw new Exception("Group not found");
        }
        
        if (group.CreatedBy != userId)
        {
            throw new Exception("Only the creator can delete the group");
        }
        
        await _groupRepository.DeleteAsync(group);
        await _groupRepository.SaveChangesAsync();
        
        return group;
    }
    
    public async Task<List<Group>> GetAllGroupsAsync(Guid userId)
    {
        var groups = await _groupRepository.GetAllForUserAsync(userId);
        return groups;
    }
    
    public async Task<Group> GetGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        
        if (group == null)
        {
            throw new Exception("Group not found");
        }
        
        return group;
    }
    
    public async Task<Group> UpdateGroupAsync(UpdateGroupDTO dto, Guid groupId, Guid userId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        
        if (group == null)
        {
            throw new Exception("Group not found");
        }
        
        if (group.CreatedBy != userId)
        {
            throw new Exception("Only the creator can update the group");
        }
        
        group.GroupName = dto.GroupName;
        group.Description = dto.Description;
        
        await _groupRepository.SaveChangesAsync();
        
        return group;
    }
}