using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserService.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Returns 400 BadRequest in RFC 7807 Error Format
        /// </summary>
        /// <param name="detail">Error Message</param>
        /// <returns></returns>
        protected IActionResult ValidationProblem(string title = "Validation Error", string detail = "One of the attributes did not meet validation rules")
        {
            return Problem(
                title: title,
                detail: detail,
                statusCode: 400,
                type: "https://httpstatuses.com/400"
            );
        }

        /// <summary>
        /// Returns 400 BadRequest in RFC 7807 Error Format
        /// </summary>
        /// <param name="errors">Validation Errors (RequestValidationException) thrown by MediatoR pipeline when executing Fluent validators on DTOs, Commands, Queries</param>
        /// <returns></returns>
        protected IActionResult ValidationProblemList(List<ValidationFailure>? errors, string title = "Validation Error", string detail = "One of the attributes did not meet validation rules")
        {
            var problemDetails = new ValidationProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = 400,
                Type = "https://httpstatuses.com/400"
            };

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                }
            }

            return BadRequest(problemDetails);
        }


        /// <summary>
        /// Returns model state errors in RFC 7807 Error Format like below
        /// {
        /// "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        /// "title": "One or more validation errors occurred.",
        /// "status": 400,
        /// "errors": {
        /// "Title": ["Title is required"],
        /// "Description": ["Description must be at least 10 characters"]
        /// }       
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected IActionResult ModelStateValidationProblem(ModelStateDictionary modelState)
        {
            return base.ValidationProblem(modelState);
        }

        /// <summary>
        /// Returns 404 NotFound in RFC 7807 Error Format
        /// </summary>
        /// <param name="detail">Error Message</param>
        /// <returns></returns>
        protected IActionResult NotFoundProblem(string title = "Resource Not Found", string detail = "Resource unavailable for the operation")
        {
            return Problem(
                title: title,
                detail: detail,
                statusCode: 404,
                type: "https://httpstatuses.com/404"
            );
        }

        /// <summary>
        /// Returns 404 Access Denied (Not Privilaged Enough to Access) in RFC 7807 Error Format
        /// </summary>
        /// <param name="detail">Error Message</param>
        /// <returns></returns>
        protected IActionResult ForbiddenProblem(string detail)
        {
            return Problem(
                title: "Access Denied",
                detail: detail,
                statusCode: 403,
                type: "https://httpstatuses.com/403"
            );
        }

        /// <summary>
        /// Returns 401 Sender of Request not identified - not authenticated
        /// </summary>
        /// <param name="detail">Error Message</param>
        /// <returns></returns>
        protected IActionResult UnauthorizedProblem(string detail)
        {
            return Problem(
                title: "Unauthorized",
                detail: detail,
                statusCode: 401,
                type: "https://httpstatuses.com/401"
            );
        }

        /// <summary>
        /// Returns 500 Internal Server Error in RFC 7807 format
        /// </summary>
        protected IActionResult InternalError(string detail = "An unexpected error occurred.")
        {
            return ProblemResponse("Internal Server Error", detail, 500);
        }

        /// <summary>
        /// Returns 503 Service Unavailable Error in RFC 7807 format
        /// If External API or Other microservice is unavaible
        /// </summary>
        /// <param name="detail">Error Message</param>
        /// <returns></returns>
        protected IActionResult ServiceUnavailableProblem(string detail = "The service is temporarily unavailable.")
        {
            return Problem(
                title: "Service Unavailable",
                detail: detail,
                statusCode: 503,
                type: "https://httpstatuses.com/503"
            );
        }

        /// <summary>
        /// Returns 409 duplicate entries or concurrency violations Error in RFC 7807 format
        /// </summary>
        /// <param name="detail">Error Message</param>
        /// <returns></returns>
        protected IActionResult ConflictProblem(string detail = "A conflict occurred while processing the request.")
        {
            return Problem(
                title: "Conflict",
                detail: detail,
                statusCode: 409,
                type: "https://httpstatuses.com/409"
            );
        }




        // RFC 7807 Error Format generic error response
        protected IActionResult ProblemResponse(
                                                string title,
                                                string? detail = null,
                                                int statusCode = 500,
                                                string? type = null)
        {
            var problem = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Type = type ?? $"https://httpstatuses.com/{statusCode}",
                Instance = HttpContext?.Request?.Path
            };

            return StatusCode(statusCode, problem);
        }

        /// <summary>
        /// Returns an HTTP 200 OK response with the specified result if it is not null; otherwise, returns an HTTP 404
        /// Not Found response with a problem detail.
        /// </summary>
        /// <typeparam name="T">The type of the result to include in the response.</typeparam>
        /// <param name="result">The result to include in the HTTP 200 OK response. If null, an HTTP 404 Not Found response is returned.</param>
        /// <param name="notFoundMessage">The message to include in the problem detail of the HTTP 404 Not Found response. Defaults to "Resource not
        /// found".</param>
        /// <returns>An <see cref="IActionResult"/> representing an HTTP 200 OK response with the specified result if <paramref
        /// name="result"/> is not null;  otherwise, an HTTP 404 Not Found response with a problem detail containing the
        /// specified <paramref name="notFoundMessage"/>.</returns>
        protected IActionResult OkOrNotFound<T>(T? result, string notFoundMessage = "Resource not found")
        {
            if (result is null)
                return NotFoundProblem(detail: notFoundMessage);

            return Ok(result);
        }

    }
}
