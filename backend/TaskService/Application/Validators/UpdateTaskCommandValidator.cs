using FluentValidation;
using TaskService.Application.Commands;

namespace TaskService.Application.Validators
{
    // FluentValidation is a popular .NET library for building strongly typed, expressive, and reusable validation rules for your models, DTOs, and commands.
    // Instead of writing a lot If statements for validations in controllers or using Attributes like Required in DTOs, models, commands, queries - we can define vadliation Rules defined using FluentValidation classes
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(status => status == "Pending" || status == "Completed")
                .WithMessage("Status must be either 'Pending' or 'Completed'.");
        }
    }
}
