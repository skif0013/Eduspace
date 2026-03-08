using CourseService.Application.Courses.Interfaces;
using CourseService.Domain.Entities;
using CourseService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Repositories;

public class CourseRatingRepository : ICourseRatingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CourseRatingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext; 
    }

    public async Task CreateRatingAsync(CourseRating rating)
    {
        await _dbContext.CourseRatings.AddAsync(rating);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<CourseRating?> GetRatingByCourseIdAndUserIdAsync(Guid courseId, Guid userId)
    {
        var rating = await _dbContext.CourseRatings
            .FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.CourseId == courseId);

        return rating;
    }

    public Task<List<CourseRating>> GetRatingsByCourseIdAsync(Guid courseId)
    {
        var ratings = _dbContext.CourseRatings
            .Where(x => x.CourseId == courseId)
            .ToListAsync();

        return ratings;
    }

    public async Task UpdateRatingAsync(CourseRating rating)
    {
        _dbContext.Update(rating);
        await _dbContext.SaveChangesAsync();
    }
}
