using SharedLib.DTOs.Journal;
using System.ComponentModel.DataAnnotations;

namespace OrchestratorService.Application.DTOs
{
    public record CreateJournalDto
    {
        [Required]
        public required AddJournalEntryDto AddJournalEntryDto { get; init; }

        [Required]
        [MinLength(1)]
        public required Guid[] LinkedTaskIds { get; init; }

    }
}
