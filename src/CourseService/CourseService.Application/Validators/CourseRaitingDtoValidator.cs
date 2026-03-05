using CourseService.Application.DTO;
using FluentValidation;

namespace CourseService.Application.Validators;

public class CourseRaitingDtoValidator : AbstractValidator<CourseRatingDTO>
{
    public CourseRaitingDtoValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
