using FluentValidation;
using MoodService.Application.Commands;

namespace MoodService.Application.Validators
{
    // UpdateMoodEntryCommand
    public class UpdateMoodEntryCommandValidator : AbstractValidator<UpdateMoodEntryCommand>
    {
        private static readonly string[] AllowedMoodTimes = { "morning", "midday", "evening" };
        private static readonly string[] AllowedMoodLevels = { "happy", "grateful", "calm", "motivated", "neutral", "tired", "stressed", "frustrated", "sad", "overwhelmed" };

        public UpdateMoodEntryCommandValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage("Mood Id must not be empty.");

            RuleFor(x => x.Day)
               .NotEmpty()
               .WithMessage("Day must not be empty.");

            RuleFor(x => x.MoodTime)            // morning, midday, evening
              .NotEmpty()
              .Must(moodTime => AllowedMoodTimes.Contains(moodTime.ToLower()))
              .WithMessage("MoodTime must be one of: Morning, MidDay, Evening.");

            RuleFor(x => x.MoodLevel)            // happy, grateful, calm, motivated, neutral, tired, stressed, frustrated, sad, overwhelmed
             .NotEmpty()
             .Must(moodLevel => AllowedMoodLevels.Contains(moodLevel.ToLower()))
             .WithMessage("MoodLevel must be one of: happy, grateful, calm, motivated, neutral, tired, stressed, frustrated, sad, overwhelmed.");
        }
    }
}
