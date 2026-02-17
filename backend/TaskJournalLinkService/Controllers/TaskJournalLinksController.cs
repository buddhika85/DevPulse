using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs.Journal;
using SharedLib.DTOs.TaskJournalLink;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using TaskJournalLinkService.Services;


namespace TaskJournalLinkService.Controllers
{
    /// <summary>
    /// This controller is intended only to call from OrchestratorService/JournalController
    /// This is used to manage linking between journals and tasks.
    /// It accomodates resolving M:M relationship between Journals and Tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TaskJournalLinksController : BaseApiController
    {
        private readonly ITaskJournalLinkService _service;
        private readonly ILogger<TaskJournalLinksController> _logger;


        public TaskJournalLinksController(ITaskJournalLinkService taskJournalLinkService, ILogger<TaskJournalLinksController> logger)
        {
            _service = taskJournalLinkService;
            _logger = logger;
        }


        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        [HttpGet("{journalId:guid}")]
        [SwaggerOperation(Summary = "Get Task Journal Links for JournalId", Description = "Returns Task Journal Links by JournalId.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(TaskJournalLinkDto[]))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetLinksByJournalIdAsync([FromRoute] Guid journalId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Attempting to find all Task Journal Links for JournalId: {JournalId} at {Time}",
                journalId, now);
            try
            {
                var links = await _service.GetLinksByJournalIdAsync(journalId, cancellationToken);
                if (links is null || links.Length == 0)
                {
                    _logger.LogWarning("No Task Journal Links found for journalId: {JournalId} at {Time}", journalId, now);
                    return NotFoundProblem("Not Found - Task Journal Links", $"No Task Journal Links found for journalId: {journalId}");
                }

                _logger.LogInformation("Found {TaskJournalLinkCount} Task Journal Links for journalId: {JournalId} at {Time}", links.Length, journalId, now);
                return Ok(links);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - Finding Task Journal Links for JournalId: {JournalId} at {Time}",
                            journalId, now);
                return InternalError($"Failed to find Task Journal Links for JournalId: {journalId} at {now}");
            }
        }


        //[Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.User)}")]
        // /TaskJournalLink
        [HttpPost]
        [SwaggerOperation(Summary = "Links journal with Task Id/s", Description = "After creating a new jounrnal it is linked with an array of task Ids")]
        [SwaggerResponse(StatusCodes.Status200OK, "Task Journal Links Created. Returns a TaskJournalLinkDto[]")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> LinkNewJournalWithTasksAsync([FromBody] LinkTasksToJournalDto linkTasksToJournalDto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Attempting to link Journal with Id: {JournalId} with TaskIds: {TaskIds} at {Time}",
                linkTasksToJournalDto.JournalId, string.Join(',', linkTasksToJournalDto.TaskIdsToLink), now);
            try
            {
                if (linkTasksToJournalDto.TaskIdsToLink is null || linkTasksToJournalDto.TaskIdsToLink.Count == 0)
                    return ValidationProblem("TaskIdsToLink cannot be empty.");

                var links = await _service.LinkNewJournalWithTasksAsync(linkTasksToJournalDto, cancellationToken);   // TaskJournalLinkDto[]
                if (links is null || links.Length == 0)
                {
                    throw new ApplicationException("Jounral link creation unsuccessful");
                }

                _logger.LogInformation("Success - Linking Journal with Id: {JournalId} with TaskIds: {TaskIds} at {Time}",
                                linkTasksToJournalDto.JournalId, string.Join(',', linkTasksToJournalDto.TaskIdsToLink), now);
                //return CreatedAtAction(nameof(GetLinksByJournalIdAsync), new { journalId = linkTasksToJournalDto.JournalId }, links);
                return Ok(links);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - Linking Journal with Id: {JournalId} with TaskIds: {TaskIds} at {Time}",
                                linkTasksToJournalDto.JournalId, string.Join(',', linkTasksToJournalDto.TaskIdsToLink), now);
                return InternalError($"Failed to link Journal with Id: {linkTasksToJournalDto.JournalId} with TaskIds: {string.Join(',', linkTasksToJournalDto.TaskIdsToLink)} at {now}");
            }
        }


        // rearrange request
        [HttpPut("rearrange-links/{journalId:guid}")]
        [SwaggerOperation(Summary = "Rearranges task journal links for a given journal Id",
                  Description = "Rearranges task journal links for a given journal Id.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Task Journal Links Rearranged")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Journal-entry not found", typeof(NotFound))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> RearrangeTaskJournalLinksAsync([FromRoute] Guid journalId,
                                                        [FromBody] HashSet<Guid> tasksToLink,
                                                        CancellationToken cancellationToken)
        {
            if (tasksToLink == null || tasksToLink.Count == 0)
            {
                _logger.LogWarning(
                    "Rearranging request must contain at least 1 task. JournalId={JournalId} at {Time}",
                    journalId, DateTime.UtcNow);

                return ValidationProblem(detail: "Rearranging request should contain at least 1 task to link.");
            }

            var links = $"[{string.Join(",", tasksToLink)}]";
            _logger.LogInformation(
                "Rearranging task journal links for JournalId={JournalId} with TaskLinks={TaskLinks} at {Time}",
                journalId, links, DateTime.UtcNow);

            try
            {
                var rearranged = await _service.RearrangeTaskJournalLinksAsync(journalId, tasksToLink, cancellationToken);

                if (rearranged)
                {
                    _logger.LogInformation(
                        "Rearranging successful for JournalId={JournalId} with TaskLinks={TaskLinks} at {Time}",
                        journalId, links, DateTime.UtcNow);

                    return NoContent();
                }

                // Service returned false → batch failed → internal error
                _logger.LogWarning(
                    "Rearranging FAILED for JournalId={JournalId}. Service returned false. TaskLinks={TaskLinks} at {Time}",
                    journalId, links, DateTime.UtcNow);

                return InternalError($"Failed to rearrange task journal links for journal-entry {journalId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception while rearranging task journal links for JournalId={JournalId} with TaskLinks={TaskLinks} at {Time}",
                    journalId, links, DateTime.UtcNow);

                return InternalError($"Unexpected error while rearranging task journal links for journal-entry {journalId}.");
            }
        }
    }
}
