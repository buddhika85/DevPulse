using FluentValidation;
using TaskService.Application.Queries;

namespace TaskService.Application.Validators
{
    public class GetTasksPaginatedQueryValidator : AbstractValidator<GetTasksPaginatedQuery>
    {
        public GetTasksPaginatedQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Status)
                 .Must(status => status == "Pending" || status == "Completed")
                 .WithMessage("Status must be either 'Pending' or 'Completed'.");
        }
    }
}
