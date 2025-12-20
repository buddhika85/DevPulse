using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // IsMoodEntryExistsCommand
    public class IsMoodEntryExistsCommandValidator : AbstractValidator<IsMoodEntryExistsCommand>
    {
        private static readonly string[] AllowedMoodTimes = { "morning", "midday", "evening" };

        public IsMoodEntryExistsCommandValidator()
        {
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
