using ContentService.Domain.Models;

namespace ContentService.Application.DTOs.GroupDTOs;

public class UpdateGroupDTO
{
    public string GroupName { get; set; } 
    
    public string Description { get; set; }
}