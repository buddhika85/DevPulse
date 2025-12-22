using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoodService.Application.Commands;
using MoodService.Application.Dtos;
using MoodService.Application.Queries;
using MoodService.Domain.ValueObjects;
using SharedLib.Application.Exceptions;
using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.Mood;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;


namespace MoodService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoodController : BaseApiController
    {
        private readonly ILogger<MoodController> _logger;
        private readonly IMediator _mediator;

        public MoodController(ILogger<MoodController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)}")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all mood-entries", Description = "Returns all the mood-entries")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<MoodEntryDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all mood-entries at {Time}", DateTime.UtcNow);
            try
            {
                var query = new GetMoodEntriesQuery();
                _logger.LogDebug("Dispatching GetMoodEntriesQuery: {@Query}", query);

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all mood-entries at {Time}", DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all mood-entries");
            }
        }
        
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get mood-entry by ID", Description = "Returns a single mood-entry by its unique identifier.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(MoodEntryDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching a mood-entry by ID: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var query = new GetMoodEntryByIdQuery(id);
                var dto = await _mediator.Send(query, cancellationToken);
                if (dto is null)
                {
                    _logger.LogWarning("mood-entry not found for ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("mood-entry found for ID: {Id}", id);
                return Ok(dto);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching a mood-entry by ID: {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching a mood-entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving the mood-entry with Id: {id}");
            }
        }

        
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("by-user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get all mood-entries by user ID", Description = "Returns all mood-entries by user Id.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<MoodEntryDto>))]       
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetMoodEntriesByUserId([FromRoute]Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching mood-entries by user ID: {UserId} at {Time}", userId, DateTime.UtcNow);
            try
            {
                var query = new GetMoodEntriesByUserIdQuery(userId);
                var dtos = await _mediator.Send(query, cancellationToken);
               

                _logger.LogInformation("Found {Count} mood-entries found for user ID: {Id}", dtos.Count, userId);
                return Ok(dtos);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching mood-entries by user ID: {UserId} at {Time}", userId, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching mood-entries by user ID: {UserId} at {Time}", userId, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all mood-entries with Id: {userId}");
            }
        }



        // checking before insert
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpGet("is-exists/{userid:guid}")]
        [SwaggerOperation(Summary = "Before an insert, checks if a mood-entry already exists by user ID, day & time", 
            Description = "Returns a true if a mood-entry already exists by user ID, day & time.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> IsMoodEntryExists([FromRoute] Guid userid, [FromQuery] DateTime day, [FromQuery] string time, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Before an insert checks if a mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", userid, day, time, DateTime.UtcNow);
            try
            {
                var query = new IsMoodEntryExistsCommand(userid, day, time);
                var isExists = await _mediator.Send(query, cancellationToken);
                
                if (isExists)
                {
                    _logger.LogInformation("A mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", userid, day, time, DateTime.UtcNow);
                    return Ok(true);
                }

                _logger.LogInformation("A mood-entry does not exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", userid, day, time, DateTime.UtcNow);
                return Ok(false);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while checking if a mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", userid, day, time, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking if a mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", userid, day, time, DateTime.UtcNow);
                return InternalError($"An error occurred while while checking if a mood-entry already exists by user ID={userid}, day={day}, & time={time} at {DateTime.UtcNow}");
            }
        }



        // checking before update
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpGet("find-other/{userid:guid}")]
        [SwaggerOperation(Summary = "Before an update, checks if a mood-entry already exists by user ID, day & time with excluded Id",
            Description = "Returns a true if a mood-entry already exists by user ID, day & time with excluded Id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> FindOtherMoodEntry([FromRoute] Guid userid, 
            [FromQuery] Guid excludedId, [FromQuery] DateTime day, [FromQuery] string time, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Before an update checks if a mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} with excluded Id={ExcludedId} at {Time}", 
                userid, day, time, excludedId, DateTime.UtcNow);
            try
            {
                var query = new FindOtherMoodEntryCommand(excludedId, userid, day, time);
                var isExists = await _mediator.Send(query, cancellationToken);

                if (isExists)
                {
                    _logger.LogInformation("An other mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} with excluded Id={ExcludedId} at {Time}", 
                        userid, day, time, excludedId, DateTime.UtcNow);
                    return Ok(true);
                }

                _logger.LogInformation("An other mood-entry does not exists by user ID={UserId}, day={Day}, & time={Session} with excluded Id={ExcludedId} at {Time}", 
                    userid, day, time, excludedId, DateTime.UtcNow);
                return Ok(false);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while checking if an other mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time} with excluded Id={ExcludedId}",
                    userid, day, time, excludedId, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking if an other mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} with excluded Id={ExcludedId} at {Time}", 
                    userid, day, time, excludedId, DateTime.UtcNow);
                return InternalError($"An error occurred while while checking if n other a mood-entry already exists by user ID={userid}, day={day}, & time={time} " +
                    $"with excluded Id={excludedId} at {DateTime.UtcNow}");
            }
        }



        
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new mood-entry", Description = "Adds a new user and returns its location. Call only if 'is-exists/{userid:guid}' returns false.")]
        [SwaggerResponse(StatusCodes.Status201Created, "User registered")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> AddMoodEntry([FromBody] AddMoodEntryDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new Mood-Entry user ID={UserId}, day={Day}, & time={Session} at {Time}", dto.UserId, dto.Day, dto.MoodTime, DateTime.UtcNow);
            try
            {
                #region check_if_entry_already_exists
                var day = dto.Day ?? DateTime.Today.Date;
                var time = dto.MoodTime ?? MoodTime.MorningSession.Value;

                _logger.LogInformation("Before an insert checks if a mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", dto.UserId, day, time, DateTime.UtcNow);

                var query = new IsMoodEntryExistsCommand(dto.UserId, day, time);
                var isExists = await _mediator.Send(query, cancellationToken);

                if (isExists)
                {
                    _logger.LogError("A mood-entry already exists by user ID={UserId}, day={Day}, & time={Session} at {Time}", dto.UserId, day, time, DateTime.UtcNow);
                    return ValidationProblem(detail: $"A mood-entry already exists by user ID={dto.UserId}, day={day}, & time={time}");
                }
                #endregion check_if_entry_already_exists


                var command = new AddMoodEntryCommand(dto.UserId, dto.Day, dto.MoodTime, dto.MoodLevel, dto.Note);
                _logger.LogDebug("AddMoodEntryCommand payload: {@Command}", command);

                var id = await _mediator.Send(command, cancellationToken);

                if (id is null)
                {
                    throw new Exception($"An error occurred while creating new Mood-Entry user ID={dto.UserId}, day={dto.Day}, & time={dto.MoodTime} at {DateTime.UtcNow}");
                }

                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while creating a new Mood-Entry user ID={UserId}, day={Day}, & time={Session} at {Time}", 
                    dto.UserId, dto.Day, dto.MoodTime, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new Mood-Entry user ID={UserId}, day={Day}, & time={Session} at {Time}",
                    dto.UserId, dto.Day, dto.MoodTime, DateTime.UtcNow);
                return InternalError($"An error occurred while creating a new Mood-Entry user ID={dto.UserId}, day={dto.Day}, & time={dto.MoodTime} at {DateTime.UtcNow}");
            }
        }



       
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPatch("update/{id:guid}")]
        [SwaggerOperation(Summary = "Update an existing mood-entry", Description = "Updates a mood-entry by ID. Call only if 'find-other/{userid:guid}' returns false.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Mood-entry updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Mood-entry not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateMoodEntry([FromRoute] Guid id, [FromBody] UpdateMoodEntryDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating mood-entry {Id} with Mood-Entry day={Day}, & time={Session} at {Time}", id, dto.Day, dto.MoodTime, DateTime.UtcNow);

            try
            {
                var command = new UpdateMoodEntryCommand(id, dto.Day, dto.MoodTime, dto.MoodLevel, dto.Note);
                var result = await _mediator.Send(command, cancellationToken);
                if (result)
                {
                    _logger.LogInformation("Successfully updated an existing Mood-Entry with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }


                _logger.LogError("Mood-Entry not found for update: {Id}", id);
                return NotFoundProblem($"Mood-Entry not found for update: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while updating an existing Mood-Entry with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Mood-Entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while updating the Mood-Entry with Id: {id}.");
            }
        }


        
        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpDelete("delete/{id:guid}")]             // [HTTPDelete] as it is a - permanent removal
        [SwaggerOperation(Summary = "Permanantly deleting an existing user mood-entry", Description = "Deleting an existing user mood-entry by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Mood-entry soft deleted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Mood-entry with Id not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteMoodEntry([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Permanantly deleting an existing user mood-entry with Id:{Id} at {Time}", id, DateTime.UtcNow);
                var command = new DeleteMoodEntryCommand(id);
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted an existing mood-entry with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }

                _logger.LogError("Mood-entry not found for deletion: {Id}", id);
                return NotFoundProblem($"Mood-entry not found for deletion: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while deleting an existing mood-entry with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mood-entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while deleting the mood-entry with Id: {id}.");
            }
        }
    }
}
