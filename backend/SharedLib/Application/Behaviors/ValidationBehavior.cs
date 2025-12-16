using FluentValidation;
using MediatR;
using SharedLib.Application.Exceptions;

namespace SharedLib.Application.Behaviors
{

    // 🧠 How This Works
    // MediatR lets you define a behavior that wraps every request.You plug in FluentValidation so that:
    //- Every incoming request(command/query) is intercepted
    //- Its matching validator is run
    //- If validation fails, it throws a RequestValidationException (custom)
    //- If it passes, the request continues to the handler

    // This pipeline behavior intercepts every MediatR request (command or query)
    // and runs FluentValidation against it before passing it to the handler.
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        // Injects all validators registered for the current request type.
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // If there are any validators for this request, run them.
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // Run all validators asynchronously and collect results.
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                // Extract all validation failures.
                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                // If any failures exist, throw a RequestValidationException.
                if (failures.Count != 0)
                    throw new RequestValidationException(failures); 
            }

            // If validation passes, continue to the next behavior or handler.
            return await next();
        }
    }


}
