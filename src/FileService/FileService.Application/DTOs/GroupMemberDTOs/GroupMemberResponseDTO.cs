namespace FileService.Application.DTOs.GroupDTOs;

public class GroupMemberResponseDTO
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
