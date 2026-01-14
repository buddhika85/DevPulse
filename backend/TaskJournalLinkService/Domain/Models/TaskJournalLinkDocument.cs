using System.Text.Json.Serialization;

namespace TaskJournalLinkService.Domain.Models
{
    // cosmos document model
    // covention - Use lowercase JSON property names in Cosmos DB
    // using JsonPropertyName as journalId the partition key and other columns in JSON documents starts with lowercase
    public record TaskJournalLinkDocument(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("taskId")] Guid TaskId,
        [property: JsonPropertyName("journalId")] string JournalId,
        [property: JsonPropertyName("createdAt")] DateTime CreatedAt
    );
}
