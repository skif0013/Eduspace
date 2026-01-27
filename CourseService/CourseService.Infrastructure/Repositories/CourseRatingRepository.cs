using CourseService.Application.Interfaces.Repositories;
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
        rating.CreatedAt = DateTime.Now;
        _dbContext.CourseRatings.AddAsync(rating);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<CourseRating?> GetRatingByCourseIdAdnUserIdAsync(Guid courseId, Guid userId)
    {
        var rating = await _dbContext.CourseRatings
            .FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.CourseId == courseId);

        return rating;
    }

    public async Task UpdateRatingAsync(CourseRating rating)
    {
        rating.UpdatedAt = DateTime.Now;
        _dbContext.Update(rating);
        await _dbContext.SaveChangesAsync();
    }
}
