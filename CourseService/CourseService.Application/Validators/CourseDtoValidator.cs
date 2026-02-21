using CourseService.Application.DTO;
using FluentValidation;

namespace CourseService.Application.Validators;

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
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Price must be greater than 0 and less than or equal to 1 000 000");

        RuleFor(x => x.AvatarURL)
            .MaximumLength(1000)
            .WithMessage("Lenght of avatarId must be less than 1000");
    }
}
