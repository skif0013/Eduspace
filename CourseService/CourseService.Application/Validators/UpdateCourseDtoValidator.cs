using CourseService.Application.DTO;
using FluentValidation;

namespace CourseService.Application.Validators;

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDTO>
{
    public UpdateCourseDtoValidator()
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
            .Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute))
            .When(x => !string.IsNullOrWhiteSpace(x.AvatarURL))
            .WithMessage("AvatarURL must be a valid URL");

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
