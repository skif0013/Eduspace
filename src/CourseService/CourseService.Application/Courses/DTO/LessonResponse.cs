using CourseService.Domain.Enums;

namespace CourseService.Application.Courses.DTO;

public class LessonResponse
{
    public Guid Id { get; set; }
    public int LessonNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? VideoURL { get; set; }
    public DateTime CreatedAt { get; set; }
}
