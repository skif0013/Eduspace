using CourseService.Application.Courses.DTO;
using FluentValidation;

namespace CourseService.Application.Courses.Validators;

public class CourseDtoValidator : AbstractValidator<CourseDTO>
{
    public CourseDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Price)
            .Equal(0)
            .When(x => x.IsFree)
            .WithMessage("Price must be 0. When Course is free");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1_000_000)
            .When(x => !x.IsFree)
            .WithMessage("Price must be greater than 0 and less than or equal to 1 000 000.  When Course is not free");


        RuleFor(x => x.AvatarURL)
            .MaximumLength(1000)
            .WithMessage("Lenght of avatarId must be less than 1000");
    }
}
