namespace ContentService.Application.DTOs.GroupDTOs;

public class GroupResponseDTO
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public string Email { get; set; } = string.Empty;
    public int MembersCount { get; set; }
    public List<GroupMemberResponseDTO> Members { get; set; } = new();
}
