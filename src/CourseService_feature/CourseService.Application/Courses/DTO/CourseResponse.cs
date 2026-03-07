using CourseService.Domain.Enums;

namespace CourseService.Application.Courses.DTO;

public class CourseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string? AvatarURL { get; set; }
    public CourseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public double AverageRating { get; set; }
    public int AmountRatings { get; set; }
}
