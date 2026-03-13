using CourseService.Application.Courses.DTO;
using FluentValidation;

namespace CourseService.Application.Courses.Validators;

public class CourseRaitingDtoValidator : AbstractValidator<CourseRatingDTO>
{
    public CourseRaitingDtoValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
