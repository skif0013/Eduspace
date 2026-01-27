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

        public async Task ArchiveCourseAsync(Course course)
        {
            course.Status = CourseStatus.Archived;
            _dbContext.Update(course);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            course.CreatedAt = DateTime.Now;
            course.Status = CourseStatus.Draft;
            _dbContext.Courses.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            return course;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            var query =  await _dbContext.Courses
                .Where(s => s.Status == CourseStatus.Published)
                .Include(x => x.CourseRatings)
                .ToListAsync();

            return query;
        }

        public async Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            var query = await _dbContext.Courses
                .Include(c => c.CourseRatings)
                .FirstOrDefaultAsync(x=>x.Id == courseId);

            return query;
        }

        public async Task PublishCourseAsync(Course course)
        {
            course.Status = CourseStatus.Published;
            _dbContext.Update(course);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            course.UpdatedAt = DateTime.Now;
            _dbContext.Update(course);
            await _dbContext.SaveChangesAsync();

            return course;
        }
    }
}
