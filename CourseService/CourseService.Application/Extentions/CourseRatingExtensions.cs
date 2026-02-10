using CourseService.Domain.Entities;

namespace CourseService.Application.Extentions
{
    public static class CourseRatingExtensions
    {
        public static (double average, int amount) CalculateRating(ICollection<CourseRating> ratings)
        {
            if (ratings == null || ratings.Count == 0)
            {
                return (0, 0);
            }

            double average = ratings.Average(x => x.Rating);
            int amount = ratings.Count;

            return (Math.Round(average, 1), amount);
        }
    }
}
