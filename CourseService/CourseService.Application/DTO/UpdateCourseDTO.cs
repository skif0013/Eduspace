namespace CourseService.Application.DTO;

public class UpdateCourseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string AvatarURL { get; set; }
}
