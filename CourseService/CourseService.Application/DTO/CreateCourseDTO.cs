namespace CourseService.Application.DTO;

public class CreateCourseDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string AvatarURL { get; set; }
}
