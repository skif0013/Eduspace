namespace ContentService.Domain.Models;

public class GroupMember
{
    public GroupMember() {}
    
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    
    public Guid UserId { get; set; }
    
    public string Email { get; set; }
    
    public string UserName { get; set; }
}