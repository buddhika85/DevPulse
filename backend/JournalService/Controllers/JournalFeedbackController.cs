using JournalService.Application.Commands.Journal;
using JournalService.Application.Commands.JournalFeedback;
using JournalService.Application.Dtos;
using JournalService.Application.Queries.Journal;
using JournalService.Application.Queries.JournalFeedback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Application.Exceptions;
using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.Journal;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace JournalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalFeedbackController : BaseApiController
    {
        private readonly ILogger<JournalFeedbackController> _logger;
        private readonly IMediator _mediator;

        public JournalFeedbackController(ILogger<JournalFeedbackController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        //Task<IReadOnlyList<JournalFeedbackDto>> GetJournalFeedbacksAsync(CancellationToken cancellationToken);        
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)}")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all journal-feedback-entries", Description = "Returns all the journal-feedback-entries")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<JournalFeedbackDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Fetching all journal-feedback-entries at {Time}", now);
            try
            {
                var query = new GetJournalFeedbacksQuery();
                _logger.LogDebug("Dispatching GetJournalFeedbacksQuery: {@Query}", query);

                var users = await _mediator.Send(query, cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all journal-feedback-entries at {Time}", now);
                return InternalError($"An error occurred while retrieving all journal-feedback-entries");
            }
        }


        //Task<JournalFeedbackDto?> GetJournalFeedbackByIdAsync(GetJournalFeedbackByIdQuery query, CancellationToken cancellationToken);        
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get journal-feedback-entry by ID", Description = "Returns a single journal-feedback-entry by its unique identifier.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(JournalFeedbackDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Fetching a journal-feedback-entry by ID: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var query = new GetJournalFeedbackByIdQuery(id);
                var dto = await _mediator.Send(query, cancellationToken);
                if (dto is null)
                {
                    _logger.LogWarning("journal-feedback-entry not found for ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("journal-feedback-entry found for ID: {Id}", id);
                return Ok(dto);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching a journal-feedback-entry by ID: {Id} at {Time}", id, now);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching a journal-feedback-entry with ID: {Id} at {Time}", id, now);
                return InternalError($"An error occurred while retrieving the journal-feedback-entry with Id: {id}");
            }
        }

        //Task<IReadOnlyList<JournalFeedbackDto>> GetJournalFeedbacksByManagerIdAsync(GetJournalFeedbacksByManagerIdQuery query, CancellationToken cancellationToken);        
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("by-manager/{managerId:guid}")]
        [SwaggerOperation(Summary = "Get all journal-feedback-entries by manager ID", Description = "Returns all journal-entries by manager Id.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<JournalFeedbackDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetJournalEntriesByManagerId([FromRoute] Guid managerId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching journal-feedback-entries by manager ID: {ManagerId} at {Time}", managerId, DateTime.UtcNow);
            try
            {
                var query = new GetJournalFeedbacksByManagerIdQuery(managerId);
                var dtos = await _mediator.Send(query, cancellationToken);


                _logger.LogInformation("Found {Count} journal-feedback-entries found for manager ID: {Id}", dtos.Count, managerId);
                return Ok(dtos);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while fetching journal-feedback-entries by manager ID: {ManagerId} at {Time}", managerId, DateTime.UtcNow);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching journal-entries by manager ID: {ManagerId} at {Time}", managerId, DateTime.UtcNow);
                return InternalError($"An error occurred while retrieving all journal-feedback-entries with managerId: {managerId}");
            }
        }

        //// business rule - journal entry can have exctly one feedback
        //Task<bool> IsFeedbackGiven(IsFeedbackGivenQuery query, CancellationToken cancellationToken);
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpGet("is-feedback-already-given/{journalId:guid}")]
        [SwaggerOperation(Summary = "Before inserting a journal feedback, checks if a journal-entry exists by journal ID",
            Description = "Returns a true if a journal-entry already exists by journal ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> IsFeedbackGiven([FromRoute] Guid journalId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Before inserting a journal-feedback, checks if a journal-feedback already exists for this journal ID={JournalId} at {Time}", journalId, now);
            try
            {
                var query = new IsFeedbackGivenQuery(journalId);
                var isExists = await _mediator.Send(query, cancellationToken);

                if (isExists)
                {
                    _logger.LogInformation("A journal-feedback do already exist by journal ID={JournalId} at {Time}", journalId, DateTime.UtcNow);
                    return Ok(true);
                }

                _logger.LogInformation("A journal-feedback do not exist by journal ID={JournalId} at {Time}", journalId, DateTime.UtcNow);
                return Ok(false);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogError(rex, "Validation failed while checking if a journal-feedback exists for journal ID={JournalId} at {Time}", journalId, now);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking if a journal-feedback exists for journal ID={JournalId} at {Time}", journalId, now);
                return InternalError($"An error occurred while checking if a journal-feedback exists for journal ID={journalId} at {now}");
            }
        }

        //// business rule - journal entry can have exctly one feedback
        //Task<Guid?> AddJournalFeedbackAsync(AddJournalFeedbackCommand command, CancellationToken cancellationToken);
        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new journal-feedback", Description = "Adds a new journal and returns its location. " +
            "   Before calling this check if a jounral feedback arelady exists for journal by calling - is-feedback-already-given/{journalId:guid}")]
        [SwaggerResponse(StatusCodes.Status201Created, "Journal-Feedback added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> AddJournalFeedback([FromBody] AddJournalFeedbackDto dto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Adding a new journal-feedback by manager Id:{ManagerId} for journalId:{JournalId} at {Time}", dto.FeedbackManagerId, dto.JounralEntryId, now);
            try
            {
                var command = new AddJournalFeedbackCommand(dto.FeedbackManagerId, dto.JounralEntryId, dto.Comment);
                _logger.LogDebug("AddJournalFeedbackCommand payload: {@Command}", command);

                var id = await _mediator.Send(command, cancellationToken);

                if (id is null)
                {
                    throw new Exception($"An error occurred while creating new journal-feedback manager Id:{dto.FeedbackManagerId} for journalId:{dto.JounralEntryId} at {now}");
                }

                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (RequestValidationException rex)
            {
                _logger.LogWarning(rex, "Validation failed while creating new journal-feedback by manager Id:{ManagerId} for journalId:{JournalId} at {Time}", 
                    dto.FeedbackManagerId, dto.JounralEntryId, now);
                return ValidationProblemList(rex.Failures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new journal-feedback with manager Id:{ManagerId} for journalId:{JournalId} at {Time}", 
                            dto.FeedbackManagerId, dto.JounralEntryId, now);
                return InternalError($"An error occurred while creating new journal-feedback manager Id:{dto.FeedbackManagerId} for journalId:{dto.JounralEntryId} at {now}");
            }
        }
    }
}
