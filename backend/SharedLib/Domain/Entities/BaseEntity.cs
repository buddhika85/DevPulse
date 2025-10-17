namespace SharedLib.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        // Optional: Add these only if ALL entities will use them
        // public DateTime CreatedAt { get; set; }
        // public DateTime UpdatedAt { get; set; }
        // public bool IsDeleted { get; set; }

    }
}
