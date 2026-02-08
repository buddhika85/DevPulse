using System.Text.Json.Serialization;

namespace TaskJournalLinkService.Domain.Models
{
    // cosmos document model
    // covention - Use lowercase JSON property names in Cosmos DB
    // using JsonPropertyName as journalId the partition key and other columns in JSON documents starts with lowercase
    //    public record TaskJournalLinkDocument(
    //    [property: JsonPropertyName("id")] Guid Id,
    //    [property: JsonPropertyName("taskId")] Guid TaskId,
    //    [property: JsonPropertyName("journalId")] string JournalId,
    //    [property: JsonPropertyName("createdAt")] DateTime CreatedAt
    //);

    //public record TaskJournalLinkDocument
    //{
    //    [JsonPropertyName("id")]
    //    public Guid Id { get; init; }

    //    [JsonPropertyName("taskId")]
    //    public Guid TaskId { get; init; }

    //    [JsonPropertyName("journalId")]
    //    public string JournalId { get; init; }

    //    [JsonPropertyName("createdAt")]
    //    public DateTime CreatedAt { get; init; }

    //    public TaskJournalLinkDocument(Guid id, Guid taskId, string journalId, DateTime createdAt)
    //    {
    //        Id = id;
    //        TaskId = taskId;
    //        JournalId = journalId;
    //        CreatedAt = createdAt;
    //    }
    //}

    //public record TaskJournalLinkDocument
    //{
    //    [Newtonsoft.Json.JsonProperty("id")]
    //    public Guid Id { get; init; }

    //    [Newtonsoft.Json.JsonProperty("id")]
    //    public Guid TaskId { get; init; }

    //    [Newtonsoft.Json.JsonProperty("id")]
    //    public string JournalId { get; init; }

    //    [Newtonsoft.Json.JsonProperty("id")]
    //    public DateTime CreatedAt { get; init; }

    //    public TaskJournalLinkDocument(Guid id, Guid taskId, string journalId, DateTime createdAt)
    //    {
    //        Id = id;
    //        TaskId = taskId;
    //        JournalId = journalId;
    //        CreatedAt = createdAt;
    //    }
    //}

    public class TaskJournalLinkDocument
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public Guid Id { get; set; }

        [Newtonsoft.Json.JsonProperty("taskId")]
        public Guid TaskId { get; set; }

        [Newtonsoft.Json.JsonProperty("journalId")]
        public string JournalId { get; set; }

        [Newtonsoft.Json.JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        public TaskJournalLinkDocument(Guid id, Guid taskId, string journalId, DateTime createdAt)
        {
            Id = id;
            TaskId = taskId;
            JournalId = journalId;
            CreatedAt = createdAt;
        }
    }

}
