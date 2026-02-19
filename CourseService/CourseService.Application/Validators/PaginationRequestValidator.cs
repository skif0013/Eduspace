using CourseService.Application.DTO;
using FluentValidation;

namespace CourseService.Application.Validators;

public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(1000)
            .WithMessage("Page must be greater than 0 and less than or equal to 1 000");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(50)
            .WithMessage("PageSize must be greater than 0 and less than or equal to 50");
    }
}
