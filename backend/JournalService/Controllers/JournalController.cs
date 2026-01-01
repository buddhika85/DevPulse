using JournalService.Application.Commands.Journal;
using JournalService.Application.Dtos;
using JournalService.Application.Queries.Journal;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Application.Exceptions;
using SharedLib.DTOs.Journal;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace JournalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalController : BaseApiController
    {
        private readonly ILogger<JournalController> _logger;
        private readonly IMediator _mediator;

        public JournalController(ILogger<JournalController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        //GetAllJournalEntriesAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)}")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all journal-entries", Description = "Returns all the journal-entries")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<JournalEntryDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all journal-entries at {Time}", DateTime.UtcNow);
            try
            {
                var query = new GetJournalEntriesQuery();
                _logger.LogDebug("Dispatching GetJournalEntriesQuery: {@Query}", query);

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all journal-entries at {Time}", DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all journal-entries");
            }
        }

        //GetJournalEntryByIdAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get journal-entry by ID", Description = "Returns a single journal-entry by its unique identifier.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(JournalEntryDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching a journal-entry by ID: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var query = new GetJournalEntryByIdQuery(id);
                var dto = await _mediator.Send(query, cancellationToken);
                if (dto is null)
                {
                    _logger.LogWarning("journal-entry not found for ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("journal-entry found for ID: {Id}", id);
                return Ok(dto);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching a journal-entry by ID: {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching a journal-entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving the journal-entry with Id: {id}");
            }
        }

        //GetJournalEntriesByUserIdAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("by-user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get all journal-entries by user ID", Description = "Returns all journal-entries by user Id.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<JournalEntryDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetJournalEntriesByUserId([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching journal-entries by user ID: {UserId} at {Time}", userId, DateTime.UtcNow);
            try
            {
                var query = new GetJournalEntriesByUserIdQuery(userId);
                var dtos = await _mediator.Send(query, cancellationToken);


                _logger.LogInformation("Found {Count} journal-entries found for user ID: {Id}", dtos.Count, userId);
                return Ok(dtos);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching journal-entries by user ID: {UserId} at {Time}", userId, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching journal-entries by user ID: {UserId} at {Time}", userId, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all journal-entries with Id: {userId}");
            }
        }

        // checking before insert
        //IsJournalEntryExistsByIdAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpGet("is-exists/{journalId:guid}")]
        [SwaggerOperation(Summary = "Before inserting a journal feedback, checks if a journal-entry exists by journal ID",
            Description = "Returns a true if a journal-entry already exists by journal ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> IsJournalEntryExistsById([FromRoute] Guid journalId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Before inserting a journal-feedback, checks if a journal-entry exists by journal ID={JournalId} at {Time}", journalId, now);
            try
            {
                var query = new IsJournalEntryExistsByIdQuery(journalId);
                var isExists = await _mediator.Send(query, cancellationToken);

                if (isExists)
                {
                    _logger.LogInformation("A journal-entry do exist by journal ID={JournalId} at {Time}", journalId, DateTime.UtcNow);
                    return Ok(true);
                }

                _logger.LogInformation("A journal-entry do not exist by journal ID={JournalId} at {Time}", journalId, DateTime.UtcNow);
                return Ok(false);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while checking if a journal-entry exists by journal ID={JournalId} at {Time}", journalId, now);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking if a journal-entry exists by journal ID={JournalId} at {Time}", journalId, now);
                return InternalError($"An error occurred while checking if a journal-entry exists by journal ID={journalId} at {now}");
            }
        }

        //AddJournalEntryAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new journal-entry", Description = "Adds a new journal and returns its location.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Journal added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> AddJournalEntry([FromBody] AddJournalEntryDto dto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Adding a new journal-entry with user Id:{UserId} Title:{Title} Content:{Content} at {Time}", dto.UserId, dto.Title, dto.Content, now);
            try
            {
                var command = new AddJournalEntryCommand(dto.UserId, dto.Title, dto.Content);
                _logger.LogDebug("AddJournalEntryCommand payload: {@Command}", command);

                var id = await _mediator.Send(command, cancellationToken);

                if (id is null)
                {
                    throw new Exception($"An error occurred while creating new journal-Entry user ID={dto.UserId}, Title:{dto.Title} Content:{dto.Content} at {now}");
                }

                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while creating a new journal-entry with user Id:{UserId} Title:{Title} Content:{Content} at {Time}", 
                    dto.UserId, dto.Title, dto.Content, now);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new journal-entry with user Id:{UserId} Title:{Title} Content:{Content} at {Time}", 
                    dto.UserId, dto.Title, dto.Content, now);
                return InternalError($"An error occurred while creating new journal-Entry user ID={dto.UserId}, Title:{dto.Title} Content:{dto.Content} at {now}");
            }
        }

        //UpdateJournalEntryAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPatch("update/{id:guid}")]
        [SwaggerOperation(Summary = "Update an existing journal-entry", Description = "Updates a journal-entry by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Journal-entry updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Journal-entry not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateJournalEntry([FromRoute] Guid id, [FromBody] UpdateJournalEntryDto dto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Updating journal-entry {Id} with journal-Entry Title={Title}, & Content={Content} at {Time}", id, dto.Title, dto.Content, now);

            try
            {
                var command = new UpdateJournalEntryCommand(dto.JournalEntryId, dto.Title, dto.Content);
                var result = await _mediator.Send(command, cancellationToken);
                if (result)
                {
                    _logger.LogInformation("Successfully updated an existing Journal-Entry with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }

                _logger.LogError("Journal-Entry not found for update: {Id}", id);
                return NotFoundProblem($"Journal-Entry not found for update: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while updating an existing Journal-Entry with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Journal-Entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while updating the Journal-Entry with Id: {id}.");
            }
        }


        //DeleteAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPatch("soft-delete/{id:guid}")]             // [HTTPDelete] as it is soft-delete not a permanent removal
        [SwaggerOperation(Summary = "Soft deleting an existing user journal-entry", Description = "Soft Deleting an existing journal-entry by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Journal-entry soft deleted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Journal-entry with Id not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> SoftDeleteJournalEntry([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Soft-deleting an existing user journal-entry with Id:{Id} at {Time}", id, DateTime.UtcNow);
                var command = new DeleteJournalEntryCommand(id);
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted an existing journal-entry with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }

                _logger.LogError("Journal-entry not found for soft-deletion: {Id}", id);
                return NotFoundProblem($"Journal-entry not found for soft-deletion: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while soft-deleting an existing journal-entry with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting journal-entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while deleting the journal-entry with Id: {id}.");
            }
        }

        //RestoreAsync
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPatch("restore/{id:guid}")]             // [HTTPDelete] as it is soft-delete not a permanent removal
        [SwaggerOperation(Summary = "Restoring an existing user journal-entry", Description = "Restoring an existing user journal-entry by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Journal-entry restored")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Journal-entry with Id not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> RestoreJournalEntry([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Restoring an existing user journal-entry with Id:{Id} at {Time}", id, DateTime.UtcNow);
                var command = new RestoreJournalEntryCommand(id);
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Successfully restored an existing journal-entry with id {Id} at {Time}", id, DateTime.UtcNow);
                    return NoContent();
                }

                _logger.LogError("Journal-entry not found for restore: {Id}", id);
                return NotFoundProblem($"Journal-entry not found for soft-deletion: {id}");
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while restoring an existing journal-entry with id {Id} at {Time}", id, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring journal-entry with ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError($"An error occurred while restoring the journal-entry with Id: {id}.");
            }
        }
    }
}
