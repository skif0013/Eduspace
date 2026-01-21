using CourseService.Domain.Abstractions;

namespace CourseService.Domain.Entities
{
    public class CourseRating : Entity
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }

        public Course Course { get; set; } = null!;
    }
}
