using Microsoft.AspNetCore.Mvc;
using SharedLib.Presentation.Controllers;

namespace OrchestratorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseApiController
    {
        // To dos
        // GET /tasks --> All tasks + linked journals
        // GET /tasks/{id} --> Task + linked journals

    }
}
