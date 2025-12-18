using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // AddMoodEntryCommand
    public class AddMoodEntryCommandValidator : AbstractValidator<AddMoodEntryCommand>
    {
        public AddMoodEntryCommandValidator()
        {
            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");

            /*
             DateTime? Day, MoodTime? MoodTime, MoodLevel? MoodLevel, string? Note => These will be set to defaults if not provided, so no need of strict validation
             */
        }
    }
}
