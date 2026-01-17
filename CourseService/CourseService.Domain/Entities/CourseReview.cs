using CourseService.Domain.Abstractions;

namespace CourseService.Domain.Entities
{
    public class CourseReview : Entity
    { 
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public int Value { get; set; }

        private CourseReview() { }
    }
}
