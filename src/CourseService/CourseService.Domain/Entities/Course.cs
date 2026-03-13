using CourseService.Domain.Abstractions;
using CourseService.Domain.Enums;

namespace CourseService.Domain.Entities;

public class Course : Entity
{
    public Guid AuthorId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public string? AvatarURL { get; set; } // ToDo
    public CourseStatus Status { get; set; }

    public ICollection<CourseRating> CourseRatings { get; set; } = new List<CourseRating>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
