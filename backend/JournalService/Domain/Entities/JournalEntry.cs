using JournalService.Domain.Events.JournalEntryEvents;
using SharedLib.Domain.Entities;

namespace JournalService.Domain.Entities
{
    public class JournalEntry : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;       

        // FK
        // we dont keep any navigational properties in entities as the reference DB tables live in other micro service specific DBs - EF do not have access to them
        public Guid UserId { get; private set; }


        // FK - 1:1 relationship
        public Guid? JournalFeedbackId { get; private set; }
        // Navigational property with same JournalDB
        public virtual JournalFeedback? JournalFeedback { get; private set; }


        public bool HasManagerFeedback => JournalFeedbackId != null;

        public bool IsDeleted { get; private set; }

        private JournalEntry() { }   // Enforces controlled instantiation via Create()

        #region domain_events

        public static JournalEntry Create(Guid userId,
                                           string title, 
                                           string content)
        {
            var journalEntry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Content = content,
                JournalFeedbackId = null,
                JournalFeedback = null,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            journalEntry.DomainEvents.Add(new JournalEntryCreatedDomainEvent(journalEntry));          

            return journalEntry;
        }

        public void Update(string title, string content)
        {         
            if (HasManagerFeedback)
                throw new InvalidOperationException("Journal already has a manager feedback. After receiving a manager feedback journal cannot be updated again.");


            var beforeUpdate = GetADeepClone();     // GetMemberwiseClone();      

            Title = title;
            Content = content;

            DomainEvents.Add(new JournalEntryUpdatedDomainEvent(beforeUpdate, this));             
        }

        public void AttachJournalFeedback(JournalFeedback journalFeedback)
        {
            if (HasManagerFeedback)
                throw new InvalidOperationException("Journal already has a manager feedback. Cannot provide another manager feedback again.");

            JournalFeedback = journalFeedback;
            JournalFeedbackId = journalFeedback.Id;            

            DomainEvents.Add(new JournalFeedbackProvidedDomainEvent(this));                       
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DomainEvents.Add(new JournalEntrySoftDeletedDomainEvent(this));
        }

        public void Restore()
        {
            IsDeleted = false;
            DomainEvents.Add(new JournalEntryRestoredDomainEvent(this));
        }

        #endregion domain_events


        // manual deep copy
        private JournalEntry GetADeepClone() => new()
        {
            Id = Id,
            UserId = UserId,
            Title = Title,
            Content = Content,
            CreatedAt = CreatedAt,
            IsDeleted = IsDeleted,
            JournalFeedbackId = JournalFeedbackId
        };

        // shallow copy, no issues as ValueObjects used
        private JournalEntry GetMemberwiseClone() => (JournalEntry)MemberwiseClone();
    }
}
