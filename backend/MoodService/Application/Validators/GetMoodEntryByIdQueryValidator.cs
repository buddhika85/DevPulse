using FluentValidation;
using MoodService.Application.Queries;

namespace MoodService.Application.Validators
{
    // GetMoodEntryByIdQuery
    public class GetMoodEntryByIdQueryValidator : AbstractValidator<GetMoodEntryByIdQuery>
    {
        public GetMoodEntryByIdQueryValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage("Mood Id must not be empty.");
        }
    }
}
