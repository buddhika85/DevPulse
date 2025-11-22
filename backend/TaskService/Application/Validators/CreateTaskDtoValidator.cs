using FluentValidation;
using TaskService.Application.Dtos;

namespace TaskService.Application.Validators
{
    // FluentValidation is a popular .NET library for building strongly typed, expressive, and reusable validation rules for your models, DTOs, and commands.
    // Instead of writing a lot If statements for validations in controllers or using Attributes like Required in DTOs, models, commands, queries - we can define vadliation Rules defined using FluentValidation classes
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Priority.ToLower())
                .NotEmpty()
                .Must(status => status == "high" || status == "medium" || status == "low")
                .WithMessage("Status must be either 'High', 'Medium' or 'Low'.");

            RuleFor(x => x.Status.ToLower())
                .NotEmpty()
                .Must(status => status == "pending" || status == "completed" || status == "notstarted")
                .WithMessage("Status must be either 'Pending', 'Completed' or 'NotStarted'.");
        }
    }
}
