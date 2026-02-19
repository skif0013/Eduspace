namespace CourseService.Application.DTO;

public class PagedCoursesResponse
{
    public List<CourseResponse> Courses { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
