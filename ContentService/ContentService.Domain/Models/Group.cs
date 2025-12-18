namespace ContentService.Domain.Models;

public class Group
{
   public Group() {}
   public Guid Id { get; set; }
   
   public string GroupName { get;  set; } = string.Empty;

   public string Description { get; set; } = string.Empty;
   
   public string Email { get; set; }
   
   public Guid CreatedBy { get; set; }
   
   public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
}