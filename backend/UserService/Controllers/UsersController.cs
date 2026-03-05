using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Domain.ValueObjects;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.Queries;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> logger, IMediator mediator) : BaseApiController
    {
        private readonly ILogger<UsersController> _logger = logger;
        private readonly IMediator _mediator = mediator;


        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Manager)}")]
        [HttpGet("team-for-manager")]
        [SwaggerOperation(Summary = "Get all team members for a manager", Description = "Returns all team members for a manager")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<IReadOnlyList<Guid>>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllForManager(CancellationToken cancellationToken, [FromQuery] bool includeDeleted = false)
        {
            var managerOid = User.FindFirst("oid")?.Value ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (string.IsNullOrEmpty(managerOid))
            {
                _logger.LogWarning("Missing managers 'oid' claim in token - Fetching all team members for a manager Failed.");
                return Unauthorized("Missing managers 'Id' value in token - Fetching all team members for a manager Failed.");
            }

            _logger.LogInformation("Fetching all team members for a manager:{ManagerId} with include Deleted: {IncludeDeleted} at {Time}", managerOid, includeDeleted, DateTime.UtcNow);
            try
            {
                var query = new GetTeamMemberGuidsForManagerQuery(includeDeleted, managerOid);
                _logger.LogDebug("Dispatching Fetching all team members: {@Query}", query);

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all team members for a manager:{ManagerId} with include Deleted: {IncludeDeleted} at {Time}", managerOid, includeDeleted, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all team members for manager with include Deleted: {includeDeleted}");
            }
        }
    }
}
