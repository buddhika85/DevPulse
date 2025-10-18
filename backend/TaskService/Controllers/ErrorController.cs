using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskService.Controllers
{   
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            var problem = new ProblemDetails                                                    // RFC 7807 Error Format
            {
                Title = "An unexpected error occurred.",
                Status = 500,
                Detail = exception?.Message,
                Instance = HttpContext.Request.Path
            };

            return StatusCode(500, problem);
        }

    }
}
