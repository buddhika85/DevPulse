using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // DeleteMoodEntryCommand
    public class DeleteMoodEntryCommandValidator : AbstractValidator<DeleteMoodEntryCommand>
    {
        public DeleteMoodEntryCommandValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage("Mood Id must not be empty.");
        }
    }
}
