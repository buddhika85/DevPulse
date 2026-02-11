using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Application.Services;
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

        // TO DO
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
