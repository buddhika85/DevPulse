using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.Commands;
using UserService.Application.Common.Exceptions;
using UserService.Application.Dtos;
using UserService.Application.Queries;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IMediator _mediator;

        public ProfileController(ILogger<ProfileController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // Get By Id
        [HttpGet("getById")]
        public IActionResult GetProfile(Guid id)
        {
            return Ok(id);
            //try
            //{

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
        }

        // Get all
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all users", Description = "Returns all the users")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<UserAccountDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all users at {Time}", DateTime.UtcNow);
            try
            {
                var query = new GetAllUsersQuery();
                _logger.LogDebug("Dispatching GetAllUsersQuery: {@Query}", query);

                var users = await _mediator.Send(query);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users at {Time}", DateTime.UtcNow);
                return InternalError("An error occurred while retrieving all users");
            }
        }

        // Get all users by role

        // Get paginated result

        // Update

        // Soft delete

        [HttpPost]
        [SwaggerOperation(Summary = "Register a new user", Description = "Registers a new user and returns its location.")]
        [SwaggerResponse(StatusCodes.Status201Created, "User registered")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> RegisterUser([FromBody]RegisterUserDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering a new User at {Time}", DateTime.UtcNow);
            try
            {
                var command = new RegisterUserCommand(dto.Email, dto.DisplayName, DateTime.UtcNow, dto.Role);
                _logger.LogDebug("RegisterUserCommand payload: {@Command}", command);

                var id = await _mediator.Send(command, cancellationToken);

                if (id is null)
                {
                    throw new Exception($"An error occurred while registering a user with email: {dto.Email} and display name: {dto.DisplayName}. Service returned null as Id.");
                }

                return CreatedAtAction(nameof(GetProfile), new { id },  null);
            }
            catch(RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while registering a user with email: {Email} and display name: {DisplayName} at {Time}", dto.Email, dto.DisplayName, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering a user with email: {Email} and display name: {DisplayName}at {Time}", dto.Email, dto.DisplayName, DateTime.UtcNow);
                return InternalError($"An error occurred while registering a user with email: {dto.Email} and display name: {dto.DisplayName}");
            }
        }

    }
}
