using Microsoft.AspNetCore.Mvc;
using SharedLib.Presentation.Controllers;

namespace OrchestratorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalsController : BaseApiController
    {

        // POST Journal - contains Jounral Info, task Id for Task jounrnal link creation
        // POST /journals


        // GET Jounral By Id - get single journal and tasks linked to it
        // GET /journals/{id}

        // Get All jounrals - gets all jounrals with linked tasks
        // GET /journals


        // Patch Jounral - partial update - Pataches jounral info, re-arranges task jounral links
        // PATCH /journals/{id}

        // Patch Jounral - soft delete journal - soft deletes jounral, soft deletes task journal links
        // PATCH /journals/{id}/delete

        // Patch Jounral - restore journal - restore jounral, restore task journal links
        // PATCH /journals/{id}/restore

    }
}
