namespace CourseService.Application.Courses.DTO;

public record CourseFinishDTO(
    string To,
    string UserName,
    Guid CourseId,
    string CourseTitle,
    DateTime FinishDate,
    decimal FinishScore
    );