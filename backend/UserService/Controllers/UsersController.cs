using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.User;
using SharedLib.Extensions;
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
            var managerOid = User.GetOid();      // getting azure AD/ entra object ID of user

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


        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpPost("by-ids")]
        [SwaggerOperation(Summary = "Get users by IDs", Description = "Returns user accounts for the provided list of user IDs.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<UserAccountDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetUsersByIds([FromBody] Guid[] userIds, CancellationToken cancellationToken, [FromQuery] bool includeDeleted = false)
        {
            if (userIds is null || userIds.Length == 0)
                return ValidationProblem(detail: "Empty userIds list provided.");

            var userIdsStr = $"[{string.Join(",", userIds)}]";

            _logger.LogInformation(
                "Fetching users by IDs: {UserIds} (IncludeDeleted: {IncludeDeleted}) at {Time}",
                userIdsStr, includeDeleted, DateTime.UtcNow);

            try
            {
                var query = new GetUsersByIdsQuery(userIds, includeDeleted);
                _logger.LogDebug("Dispatching query: {@Query}", query);

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching users by IDs: {UserIds} (IncludeDeleted: {IncludeDeleted}) at {Time}",
                    userIdsStr, includeDeleted, DateTime.UtcNow);

                return InternalError("An error occurred while retrieving users by IDs.");
            }
        }
    }
}
