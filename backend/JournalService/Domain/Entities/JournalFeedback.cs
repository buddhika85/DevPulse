using JournalService.Domain.Events.JournalFeedbackEvents;
using SharedLib.Domain.Entities;

namespace JournalService.Domain.Entities
{
    public class JournalFeedback : BaseEntity
    {
        public string Comment { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public bool SeenByUser { get; private set; }

        // FK
        public Guid JournalEntryId { get; private set; }
        // Navigational property with same JournalDB
        public virtual JournalEntry? JournalEntry { get; private set; }

        // FK
        // we dont keep any navigational properties in entities as the reference DB tables live in other micro service specific DBs - EF do not have access to them
        public Guid FeedbackManagerId { get; private set; }


        private JournalFeedback() { }   // Enforces controlled instantiation via Create()

        #region domain_events

        public static JournalFeedback Create(Guid jounralEntryId,
                                            Guid feedbackManagerId,
                                            string comment)
        {
            var journalFeedback = new JournalFeedback
            {
                Id = Guid.NewGuid(),
                Comment = comment,
                JournalEntryId = jounralEntryId,
                FeedbackManagerId = feedbackManagerId,
                CreatedAt = DateTime.UtcNow
            };

            journalFeedback.DomainEvents.Add(new JournalFeedbackCreatedDomainEvent(journalFeedback));          // TO DO

            return journalFeedback;
        }

        public void MarkAsSeen()
        {
            SeenByUser = true;
            DomainEvents.Add(new JournalFeedbackMarkAsSeenDomainEvent(this));          // TO DO
        }

        // for simplicity 
        // journal feedback cannot be deleted or updated in this version        

        #endregion domain_events
    }
}
