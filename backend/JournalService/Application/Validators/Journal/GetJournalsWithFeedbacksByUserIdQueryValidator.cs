using FluentValidation;
using JournalService.Application.Queries.Journal;

namespace JournalService.Application.Validators.Journal
{
    public class GetJournalsWithFeedbacksByUserIdQueryValidator : AbstractValidator<GetJournalsWithFeedbacksByUserIdQuery>
    {
        public GetJournalsWithFeedbacksByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");
        }
    }

    
}
