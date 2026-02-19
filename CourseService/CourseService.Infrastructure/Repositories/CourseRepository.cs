using CourseService.Application.DTO;
using CourseService.Application.Interfaces.Repositories;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CourseRepository(ApplicationDbContext context)
        {
            _dbContext = context;       
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            await _dbContext.Courses.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            return course;
        }

        public async Task<PagedResult<Course>> GetPagedCoursesAsync(int page, int pageSize)
        {
            var query = _dbContext.Courses
                .Where(s => s.Status == CourseStatus.Published);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(x => x.CourseRatings)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Course>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            var query = await _dbContext.Courses
                .Include(c => c.CourseRatings)
                .FirstOrDefaultAsync(x=>x.Id == courseId);

            return query;
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _dbContext.Update(course);
            await _dbContext.SaveChangesAsync();
        }
    }
}
