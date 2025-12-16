using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Presentation.Controllers;
using System.Diagnostics;

namespace MoodService.Controllers
{   
    
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]    
    public class ErrorController : BaseApiController
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

      
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet("/error")]        
        public IActionResult HandleError()
        {            
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            _logger.LogError(exception, "Unhandled exception at {Path}", HttpContext.Request.Path);

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = 500,
                Detail = exception?.Message,
                Instance = HttpContext.Request.Path
            };

            problem.Extensions["traceId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            problem.Extensions["timestamp"] = DateTime.UtcNow;

            return StatusCode(500, problem);                                                                // RFC 7807 Error Format
        }
    }

}
