using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // IsMoodEntryExistsCommand
    public class IsMoodEntryExistsCommandValidator : AbstractValidator<IsMoodEntryExistsCommand>
    {
        private static readonly string[] AllowedMoodTimes = { "morningsession", "middaysession", "eveningsession" };

        public IsMoodEntryExistsCommandValidator()
        {
            RuleFor(x => x.UserId)
               .NotEmpty()
               .WithMessage("User Id must not be empty.");

            RuleFor(x => x.Day)
               .NotEmpty()
               .WithMessage("Day must not be empty.");

            RuleFor(x => x.MoodTime)            // morningsession, middaysession, eveningsession
             .NotEmpty()
             .Must(moodTime => AllowedMoodTimes.Contains(moodTime.ToLower()))
             .WithMessage("MoodTime must be one of: MorningSession, MidDaySession, EveningSession.");
        }
    }
}
