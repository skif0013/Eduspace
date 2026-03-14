using CourseService.Application.Courses.DTO;
using FluentValidation;

namespace CourseService.Application.Courses.Validators;

public class LessonDtoValidator : AbstractValidator<LessonDTO>
{
    public LessonDtoValidator()
    {
        RuleFor(x => x.LessonNumber)
            .GreaterThan(0)
            .LessThanOrEqualTo(1_000)
            .WithMessage("Lesson Number must be greater than 0 and less than or equal to 1 000");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.VideoUrl)
            .MaximumLength(1000)
            .WithMessage("Lenght of videoId must be less than 1000");
    }
}
