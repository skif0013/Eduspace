using CourseService.Application.Courses.Interfaces;
using CourseService.Domain.Entities;
using CourseService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LessonRepository(ApplicationDbContext dBcontext)
    {
        _dbContext = dBcontext;
    }

    public async Task<Lesson> CreateLessonAsync(Lesson lesson)
    {
        await _dbContext.Lessons.AddAsync(lesson);
        await _dbContext.SaveChangesAsync();

        return lesson;
    }

    public async Task<Lesson?> GetLessonByIdAsync(Guid lessonId)
    {
        var query = await _dbContext.Lessons
            .FirstOrDefaultAsync(x => x.Id == lessonId);

        return query;
    }

    public async Task<List<Lesson>> GetLessonsAsync(Guid courseId)
    {
        var items = await _dbContext.Lessons
            .AsNoTracking()
            .Where(x => x.CourseId == courseId)
            .ToListAsync();

        return items;
    }

    public async Task UpdateLessonAsync(Lesson lesson)
    {
        _dbContext.Lessons.Update(lesson);
        await _dbContext.SaveChangesAsync();
    }
}
