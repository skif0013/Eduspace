namespace CourseService.Application.Courses.DTO;

public class LessonDTO
{
    public int LessonNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? VideoUrl { get; set; }
}
