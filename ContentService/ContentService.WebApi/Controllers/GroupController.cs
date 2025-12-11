using ContentService.Application.Contracts;
using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IGroupMemberService _groupMemberService;
    private readonly IGroupService _groupService;
    private readonly MappingService _mappingService;

    public GroupController(IGroupMemberService groupMemberService, IGroupService groupService, MappingService mappingService)
    {
        _groupService = groupService;
        _groupMemberService = groupMemberService;
        _mappingService = mappingService;
    }
    
    [HttpGet("get-all-groups")]
    public async Task<IActionResult> GetAllGroups(Guid userId)
    {
        var groups = await _groupService.GetAllGroupsAsync(userId);

        return Ok(_mappingService.ToGroupResponseList(groups));
    }

    [HttpGet("{groupId}")]
    public async Task<IActionResult> GetGroup(Guid groupId, Guid userId)
    {
        var group = await _groupService.GetGroupAsync(groupId, userId);
        return Ok(_mappingService.ToGroupResponseDTO(group)); 
    }

    [HttpPost("CreateGroup")]
    public async Task<IActionResult> CreateGroup([FromBody] CreatingGroupRequestDTO requestDto,[FromHeader(Name = "X-User_id")] Guid userId, [FromHeader(Name = "X-User-Email")] string email)
    {
        var group = await _groupService.CreateGroupAsync(requestDto, userId, email);
        
        var responseDto = _mappingService.ToGroupResponseDTO(group);
        
        return Ok(responseDto);
    }

    [HttpPut("update/{groupId}")]
    public async Task<IActionResult> UpdateGroup([FromRoute] Guid groupId, [FromBody] UpdateGroupDTO dto, [FromHeader(Name = "X-User-Id")]Guid userId)
    {
        var updated = await _groupService.UpdateGroupAsync(dto, groupId, userId);
        return Ok(_mappingService.ToUpdateGroupDTO(updated));
    }

    [HttpDelete("{groupId}")]
    public async Task<IActionResult> DeleteGroup(Guid groupId,Guid userId)
    {
        var deleted = await _groupService.DeleteGroupAsync(groupId, userId);
        return Ok(_mappingService.ToGroupResponseDTO(deleted));
    }

    [HttpPost("{groupId}/members")]
    public async Task<IActionResult> AddMemberToGroup(Guid groupId, [FromBody] AddGroupMemberRequestDTO requestDto,Guid UserId)
    {
        var groupMember = await _groupMemberService.AddMemberToGroupAsync(requestDto, groupId,UserId);
        return Ok(_mappingService.ToGroupMemberResponseDTO(groupMember));
    }

    [HttpDelete("{groupId}/members/{memberUserId}")]
    public async Task<IActionResult> RemoveMemberFromGroup(Guid groupId, Guid memberUserId)
    {
        var removed = await _groupMemberService.RemoveMemberFromGroupAsync(groupId, memberUserId);
        return Ok(_mappingService.ToGroupMemberResponseDTO(removed));
    }

    [HttpGet("{groupId}/members/{memberUserId}")]
    public async Task<IActionResult> GetGroupMemberAsync(Guid groupId, Guid memberUserId)
    {
        var member = await _groupMemberService.GetGroupMemberAsync(groupId, memberUserId);
        return Ok(_mappingService.ToGroupMemberResponseDTO(member));
    }
}