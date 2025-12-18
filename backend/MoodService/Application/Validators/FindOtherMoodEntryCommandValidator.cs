using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // FindOtherMoodEntryCommand
    public class FindOtherMoodEntryCommandValidator : AbstractValidator<FindOtherMoodEntryCommand>
    {
        private static readonly string[] AllowedMoodTimes = { "morningsession", "middaysession", "eveningsession" };

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

            RuleFor(x => x.MoodTime)            // morningsession, middaysession, eveningsession
             .NotEmpty()
             .Must(moodTime => AllowedMoodTimes.Contains(moodTime.ToLower()))
             .WithMessage("MoodTime must be one of: MorningSession, MidDaySession, EveningSession.");
        }
    }
}
