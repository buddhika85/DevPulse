using MediatR;

namespace SharedLib.Domain.Entities
{
    public abstract class BaseEntity
    {
        #region domain events
        public List<INotification> DomainEvents { get; } = new();                   // any domain events raised like created, updated...etc
        public void ClearDomainEvents() => DomainEvents.Clear();                    // clean up any domain events
        #endregion domain events

        public Guid Id { get; set; }

        // Optional: Add these only if ALL entities will use them
        // public DateTime CreatedAt { get; set; }
        // public DateTime UpdatedAt { get; set; }
        // public bool IsDeleted { get; set; }

    }
}
