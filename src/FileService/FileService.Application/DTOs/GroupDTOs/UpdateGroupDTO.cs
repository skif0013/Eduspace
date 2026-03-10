using FileService.Domain.Models;

namespace FileService.Application.DTOs.GroupDTOs;

public class UpdateGroupDTO
{
    public string GroupName { get; set; } 
    
    public string Description { get; set; }
}