using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.User;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Application.Commands;
using UserService.Application.Common.Enums;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Models;
using UserService.Application.Dtos;
using UserService.Application.Queries;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;

        public ProfileController(ILogger<ProfileController> logger, IMediator mediator, ITokenService tokenService)
        {
            _logger = logger;
            _mediator = mediator;
            _tokenService = tokenService;
        }

        // Get By Id
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get user by ID", Description = "Returns a single user by its unique identifier.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UserAccountDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching a user by ID: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var query = new GetUserByIdQuery(id);
                var dto = await _mediator.Send(query, cancellationToken);
                if (dto is null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("User found for ID: {Id}", id);
                return Ok(dto);
            }
            catch(RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching a user by ID: {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching a user ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving the user with Id: {id}");
            }
        }

        // Get all
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all users", Description = "Returns all the users")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<UserAccountDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken, [FromQuery] bool includeDeleted = false)
        {
            _logger.LogInformation("Fetching all users with include Deleted: {IncludeDeleted} at {Time}", includeDeleted, DateTime.UtcNow);
            try
            {
                var query = new GetAllUsersQuery(includeDeleted);
                _logger.LogDebug("Dispatching GetAllUsersQuery: {@Query}", query);

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users with include Deleted: {IncludeDeleted} at {Time}", includeDeleted, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all users with include Deleted: {includeDeleted}");
            }
        }

        // Get all users by role
        [HttpGet("by-role")]
        [SwaggerOperation(Summary = "Get users by role", Description = "Returns users by their role.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<UserAccountDto>))]      
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllByRole(string role, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching users by role: {Role} at {Time}", role, DateTime.UtcNow);

                var query = new GetAllUsersByRoleQuery(UserRole.From(role));
                var dtos = await _mediator.Send(query, cancellationToken);
                _logger.LogInformation("Fetched {Count} users by role: {Role} at {Time}", dtos.Count, role, DateTime.UtcNow);
                
                return Ok(dtos);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching users by role: {Role} at {Time}", role, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users with role: {Role} at {Time}", role, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving the users with role: {role}");
            }
        }

        // Get paginated result
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Filter and paginate users", Description = "Returns a paginated list of users based on filter criteria.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(PaginatedResult<UserAccountDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string? email,
            [FromQuery] string? displayName, 
            [FromQuery] string? role, 
            [FromQuery] UserSortField? sortBy, 
            [FromQuery] bool sortDescending, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 5)
        {
            try
            {
                _logger.LogInformation("Searching for paginated users at {Time} with filters: Email={Email}, DisplayName={DisplayName}, Role={Role}",
                                            DateTime.UtcNow, email, displayName, role);
                _logger.LogInformation("Pagination: PageNumber={PageNumber}, PageSize={PageSize}, SortBy={SortBy}, Descending={SortDescending}",
                    pageNumber, pageSize, sortBy, sortDescending);


                var query = new GetUsersPaginatedQuery(email, displayName, role, pageNumber, pageSize, sortBy, sortDescending);
                var pagedResult = await _mediator.Send(query, HttpContext.RequestAborted);
                return Ok(pagedResult);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed on searching for paginated users at {Time} with filters: Email={Email}, DisplayName={DisplayName}, Role={Role}",
                                        DateTime.UtcNow, email, displayName, role);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on searching for paginated users at {Time} with filters: Email={Email}, DisplayName={DisplayName}, Role={Role}",
                                        DateTime.UtcNow, email, displayName, role);
                return InternalError($"Error on searching for paginated users at {DateTime.UtcNow} with filters: Email={email}, DisplayName={displayName}, Role={role}");
            }
        }


        // Partial update - so patch is used, for full updates [HTTPPut] needs to be used
        [HttpPatch("update/{id:guid}")]
        [SwaggerOperation(Summary = "Update an existing user", Description = "Updates a user by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateUser([FromRoute]Guid id, [FromBody]UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating user {Id} with Email={Email}, DisplayName={DisplayName}, Role={Role}, ManagerId={ManagerId}",
                                id, updateUserDto.Email, updateUserDto.DisplayName, updateUserDto.Role, updateUserDto.ManagerId);
                UserRole? role = null;
                if (updateUserDto.Role is not null)
                {
                    role = UserRole.From(updateUserDto.Role);
                    _logger.LogInformation("Updating user with id={Id}: role string:{RoleString} mapped to role: {Role} at {Time}", id, updateUserDto.Role, role, DateTime.UtcNow);
                }

                var command = new UpdateUserCommand(id, updateUserDto.Email, updateUserDto.DisplayName, role, updateUserDto.ManagerId);
                var result = await _mediator.Send(command, cancellationToken);
                if (result)
                {
                    _logger.LogInformation("Successfully updated an existing User with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }
                

                 _logger.LogError("User/Manager not found for update: {Id}", id);
                return NotFoundProblem($"User not found for update: {id}");
            }
            catch(RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while updating an existing User with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while updating the user with Id: {id}.");
            }
        }

        // Soft delete
        [HttpPatch("soft-delete/{id:guid}")]             // not [HTTPDelete] as it is not a - permanent removal
        [SwaggerOperation(Summary = "Soft deleting an existing user", Description = "Soft deletes a user by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User soft deleted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> SoftDeleteUser([FromRoute]Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Soft deleting user with Id:{Id} at {Time}", id, DateTime.UtcNow);
                var command = new DeleteUserCommand(id);
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Successfully soft deleted an existing User with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }

                _logger.LogError("User not found for soft deletion: {Id}", id);
                return NotFoundProblem($"User not found for soft deletion: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while soft deleting an existing User with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting user with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while soft deleting the user with Id: {id}.");
            }
        }


        // restore
        [HttpPatch("restore/{id:guid}")]
        [SwaggerOperation(Summary = "Restoring an existing user", Description = "Restoring a soft deleted user by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User restored")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> RestoreUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Restoring soft deleted user with Id:{Id} at {Time}", id, DateTime.UtcNow);
                var command = new RestoreUserCommand(id);
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Successfully restored an existing User with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }

                _logger.LogError("User not found for restoration: {Id}", id);
                return NotFoundProblem($"User not found for restoration: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while restoring an existing User with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring user with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while restoring the user with Id: {id}.");
            }
        }

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

                return CreatedAtAction(nameof(GetById), new { id },  null);
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


        // Secure Test End point to test Az Microsft Entral External ID
        [Authorize(Policy = "ValidToken")]
        [HttpGet("secure-ping")]
        [SwaggerOperation(Summary = "Entra JWT validation test", Description = "Returns success if JWT token is valid.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Token is valid")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid token")]
        public IActionResult SecurePing()
        {
            return Ok("Token is valid. Access granted.");
        }

        //[Authorize(Policy = "ValidToken")]
        [Authorize(AuthenticationSchemes = "EntraJwt")]
        [HttpGet("me")]
        [SwaggerOperation(Summary = "Get current user", Description = "Returns the profile of the authenticated user by talking with Microsoft Entra External ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UserAccountDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid token")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            //foreach (var claim in User.Claims)
            //{
            //    _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
            //    Console.WriteLine($"--> Claim: {claim.Type} = {claim.Value}");
            //}


            var objectId = User.FindFirst("oid")?.Value ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (string.IsNullOrEmpty(objectId))
            {
                _logger.LogWarning("Missing 'oid' claim in token.");
                return Unauthorized("Missing user identifier.");
            }

            _logger.LogInformation("Initiating fetch for current user with OID: {Oid} at {Time}", objectId, DateTime.UtcNow);

            try
            {
                var query = new GetUserByObjectIdQuery(objectId);
                _logger.LogDebug("Dispatching GetUserByObjectIdQuery: {@Query}", query);

                var dto = await _mediator.Send(query, cancellationToken);

                if (dto is null)
                {
                    _logger.LogWarning("User not found for OID: {Oid} at {Time}", objectId, DateTime.UtcNow);
                    return NotFound();
                }

                // 🔑 Generate app token enriched with role
                var role = dto.UserRole;                                                        
                var appToken = _tokenService.GenerateAppToken(User, role, cancellationToken);

                _logger.LogInformation("Successfully retrieved user profile and issued app token for OID: {Oid} at {Time}", objectId, DateTime.UtcNow);

                // 📦 Return both DTO and token
                var response = new UserProfileResponseDto
                {
                    User = dto,
                    DevPulseJwToken = appToken
                };

                _logger.LogInformation("Successfully retrieved user profile for OID: {Oid} at {Time}", objectId, DateTime.UtcNow);
                return Ok(response);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while fetching user profile for OID: {Oid} at {Time}", objectId, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching user profile for OID: {Oid} at {Time}", objectId, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving the user profile for OID: {objectId}");
            }
        }
    }
}
