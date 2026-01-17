using CourseService.Domain.Abstractions;
using CourseService.Domain.Enums;

namespace CourseService.Domain.Entities;

public class Course : Entity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string? AvatarURL { get; set; }
    public CourseStatus Status { get; set; }

    private Course() { }
}
