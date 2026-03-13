using CourseService.Domain.Abstractions;

namespace CourseService.Domain.Entities;

public class Lesson : Entity
{
    public Guid CourseId { get; set; }
    public int LessonNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? VideoUrl { get; set; }

    public Course Course { get; set; } = null!;
}
