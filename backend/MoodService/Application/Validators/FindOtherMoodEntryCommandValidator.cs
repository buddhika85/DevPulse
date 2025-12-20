using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // FindOtherMoodEntryCommand
    public class FindOtherMoodEntryCommandValidator : AbstractValidator<FindOtherMoodEntryCommand>
    {
        private static readonly string[] AllowedMoodTimes = { "morning", "midday", "evening" };

        public FindOtherMoodEntryCommandValidator()
        {
            RuleFor(x => x.ExcludeId)
               .NotEmpty()
               .WithMessage("Exclude mood Id must not be empty.");

            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");

            RuleFor(x => x.Day)
               .NotEmpty()
               .WithMessage("Day must not be empty.");

            RuleFor(x => x.MoodTime)            // morning, midday, evening
              .NotEmpty()
              .Must(moodTime => AllowedMoodTimes.Contains(moodTime.ToLower()))
              .WithMessage("MoodTime must be one of: Morning, MidDay, Evening.");
        }
    }
}
