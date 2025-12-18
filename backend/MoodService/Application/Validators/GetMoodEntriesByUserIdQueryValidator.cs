using FluentValidation;
using MoodService.Application.Queries;

namespace MoodService.Application.Validators
{
    // GetMoodEntriesByUserIdQuery
    public class GetMoodEntriesByUserIdQueryValidator : AbstractValidator<GetMoodEntriesByUserIdQuery>
    {
        public GetMoodEntriesByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");
        }
    }
}
