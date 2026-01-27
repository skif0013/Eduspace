using CourseService.Domain.Enums;

namespace CourseService.Application.DTO;

public class UpdateCourseDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string AvatarURL { get; set; }
    public CourseStatus Status { get; set; }
}
