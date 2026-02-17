using System.ComponentModel.DataAnnotations;

namespace SharedLib.DTOs.Journal
{
    /// <summary>
    /// used for making links between Journal Id and TaskIds
    /// </summary>
    public record LinkTasksToJournalDto
    {
        [Required]
        public required Guid JournalId { get; init; }

        [Required]
        [MinLength(1)]
        public required HashSet<Guid> TaskIdsToLink { get; init; }
    }
}
