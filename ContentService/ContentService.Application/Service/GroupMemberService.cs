using System.Security.Claims;
using ContentService.Application.Contracts;
using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Domain.Models;


namespace ContentService.Application.Service;

public class GroupMemberService : IGroupMemberService
{
   private readonly IGroupRepository _groupRepository;
   private readonly IGroupMemberRepository _groupMemberRepository;
   
   public GroupMemberService(IGroupMemberRepository groupMemberRepository,IGroupRepository groupRepository)
   {
      _groupRepository = groupRepository;
      _groupMemberRepository = groupMemberRepository;
   }
   
   
   public async Task<GroupMember> AddMemberToGroupAsync( AddGroupMemberRequestDTO requestDto,Guid groupId, Guid userId)
   {
      var group = await _groupRepository.GetByIdAsync(groupId);
      
      if (group == null)
      {  
         throw new Exception("Group not found");
      }

      var existingMember = await _groupMemberRepository.GetAsync(groupId, userId);

      if (group.Members.Any(m => m.UserId == userId))
      {
         throw new Exception("Group member already exists");
      }

      var groupMember = new GroupMember
      {
         GroupId = groupId,
         UserId = userId,
         UserName = requestDto.UserName,
         Email = requestDto.Email
      };
      
      await _groupMemberRepository.AddAsync(groupMember);
      await _groupMemberRepository.SaveChangesAsync();
      
      return groupMember;
   }
   
   public async Task<GroupMember> RemoveMemberFromGroupAsync(Guid groupId, Guid userId)
   {
      var member = await _groupMemberRepository.DeleteAsync(userId, groupId);
      
      await _groupMemberRepository.SaveChangesAsync();
      
      return member;
   }
   public async Task<GroupMember> GetGroupMemberAsync(Guid groupId, Guid userId)
   {
      var member = await _groupMemberRepository.GetAsync(userId, groupId);
      
      if (member == null)
      {
         throw new Exception("Group member not found");
      }
      
      return member;
   }
}