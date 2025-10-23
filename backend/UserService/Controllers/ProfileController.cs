using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseApiController
    {
        [HttpGet]
        public IActionResult GetProfile()
        {
            return Ok(new { Message = "Authenticated access to user profile." });
        }

    }
}
