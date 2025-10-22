using FluentValidation.Results;

namespace UserService.Application.Common.Exceptions
{
    /// <summary>
    /// Custom exception used to carry FluentValidation failures from the MediatR pipeline.
    /// </summary>

    public class RequestValidationException : Exception
    {
        /// <summary>
        /// List of validation failures for the request.
        /// </summary>
        public List<ValidationFailure> Failures { get; }

        public RequestValidationException(IEnumerable<ValidationFailure> failures)
            : base("Request validation failed")
        {
            Failures = failures.ToList();
        }

    }
}
