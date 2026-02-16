using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Application.Services;
using SharedLib.Application.Exceptions;
using SharedLib.DTOs.Journal;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace OrchestratorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalsController : BaseApiController
    {
        private readonly IJournalService _journalService;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ILogger<JournalsController> _logger;

        public JournalsController(IJournalService journalService, IOutputCacheStore outputCacheStore, ILogger<JournalsController> logger)
        {
            _journalService = journalService;
            _outputCacheStore = outputCacheStore;
            _logger = logger;
        }


        // POST Journal - contains Jounral Info, task Id for Task jounrnal link creation
        // POST /journals
        [HttpPost]
        [SwaggerOperation(Summary = "Adds a new journal-entry with linked Tasks", Description = "Adds a new journal with linked tasks and returns its location.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Journal added")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> AddJournalWithTaskLinks([FromBody] CreateJournalDto dto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Adding a new journal-entry with user Id:{UserId} Title:{Title} and tasks:{TasksList} at {Time}", 
                dto.AddJournalEntryDto.UserId, dto.AddJournalEntryDto.Title, string.Join(',', dto.LinkedTaskIds), now);
            try
            {
                var journalId = await _journalService.AddJournalEntryWithTaskLinksAsync(dto, cancellationToken);
                if (journalId is null)
                {
                    throw new InvalidOperationException($"An error occurred while creating new journal-Entry user ID={dto.AddJournalEntryDto.UserId}, Title:{dto.AddJournalEntryDto.Title} at {now}");
                }

                return CreatedAtAction(nameof(GetJounralWithTaskLinksById), new { id = journalId }, new { journalId });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation failed when creating a journal with title:{JournalTitle} for user by ID: {UserId} at {Time}", dto.AddJournalEntryDto.Title, dto.AddJournalEntryDto.UserId, now);
                return ValidationProblem(detail: ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating a new journal with task journal links");
                return InternalError(detail: ex.Message);
            }
        }

       
        // GET Jounral By Id - get single journal and tasks linked to it
        // GET /journals/{id}
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get journal-entry by ID", Description = "Returns a single journal-entry with its TaskJournalLinks")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(JournalEntryDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetJounralWithTaskLinksById(Guid id, CancellationToken cancellationToken)
        {
            try
            {               
                var journal = await _journalService.GetJournalByIdAsync(id, cancellationToken);
                if (journal is null)
                    return NotFound();
                
                return Ok(journal);
            }
            catch (ArgumentException ex)
            {
                return ValidationProblem(detail: ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving journal with Id {JournalId} and tasks linked to it", id);
                return InternalError(detail: ex.Message);
            }
        }


        // TO DO - does not need for MVP
        // Get All jounrals - gets all jounrals with linked tasks
        // GET /journals     

        // Patch Jounral - partial update - Pataches jounral info, re-arranges task jounral links
        // PATCH /journals/{id}
        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Updates journal and rearranges TaskJournalLinks", Description = "Updates journal and replaces TaskJournalLinks")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Journal with ID Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateJournalWithTaskLinks([FromRoute] Guid id,
                                                                        [FromBody] UpdateJournalDto dto,
                                                                        CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var links = $"[{string.Join(",", dto.LinkedTaskIds)}]";

            _logger.LogInformation(
                "Received request to update JournalId={JournalId} with Title={Title} and TaskLinks={TaskLinks} at {Time}",
                id, dto.UpdateJournalEntryDto.Title, links, now);

            try
            {
                // Validation: route ID must match DTO ID
                if (id != dto.UpdateJournalEntryDto.JournalEntryId)
                {
                    _logger.LogWarning(
                        "Route ID mismatch for JournalId={JournalId}. DTO JournalEntryId={DtoId} at {Time}",
                        id, dto.UpdateJournalEntryDto.JournalEntryId, now);

                    return ValidationProblem("Route Id parameter must match UpdateJournalEntryDto.JournalEntryId.");
                }

                // Validation: LinkedTaskIds cannot be null
                if (dto.LinkedTaskIds is null)
                {
                    _logger.LogWarning(
                        "LinkedTaskIds is null for JournalId={JournalId} at {Time}",
                        id, now);

                    return ValidationProblem("LinkedTaskIds cannot be null.");
                }

                // Check journal existence
                var exists = await _journalService.IsJournalEntryExistsByIdAsync(id, cancellationToken);
                if (!exists)
                {
                    _logger.LogWarning(
                        "JournalId={JournalId} not found. Update aborted at {Time}",
                        id, now);

                    return NotFoundProblem($"Journal-entry not found for update: {id}");
                }

                // Perform update + link rearrangement
                _logger.LogInformation(
                    "Attempting update + link rearrangement for JournalId={JournalId} at {Time}",
                    id, now);

                var result = await _journalService.TryUpdateJournalAndLinksAsync(dto, cancellationToken);

                if (!result)
                {
                    _logger.LogWarning(
                        "Update or link rearrangement FAILED for JournalId={JournalId} at {Time}",
                        id, now);

                    return InternalError($"Failed to update journal {id}");
                }

                _logger.LogInformation(
                    "Successfully updated JournalId={JournalId} with Title={Title} and TaskLinks={TaskLinks} at {Time}",
                    id, dto.UpdateJournalEntryDto.Title, links, now);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating JournalId={JournalId} at {Time}",
                    id, now);

                return InternalError($"An error occurred while updating the journal-entry with Id: {id}.");
            }
        }

        // TO DO - does not need for MVP
        // Patch Jounral - soft delete journal - soft deletes jounral, soft deletes task journal links
        // PATCH /journals/{id}/delete

        // TO DO - does not need for MVP
        // Patch Jounral - restore journal - restore jounral, restore task journal links
        // PATCH /journals/{id}/restore
    }
}
