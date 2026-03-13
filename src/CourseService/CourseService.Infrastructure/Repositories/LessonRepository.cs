using CourseService.Infrastructure.Data;

namespace CourseService.Infrastructure.Repositories;

public class LessonRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LessonRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }
}
