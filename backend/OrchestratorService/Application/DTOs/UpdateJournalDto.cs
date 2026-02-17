using SharedLib.DTOs.Journal;
using System.ComponentModel.DataAnnotations;

namespace OrchestratorService.Application.DTOs
{
    public record UpdateJournalDto
    {
        [Required]
        public required UpdateJournalEntryDto UpdateJournalEntryDto { get; init; }

        [Required]
        [MinLength(1)]
        public required HashSet<Guid> LinkedTaskIds { get; init; }

    }
}
