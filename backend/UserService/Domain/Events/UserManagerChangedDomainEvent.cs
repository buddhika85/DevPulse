using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserManagerChangedDomainEvent : INotification 
    {
        public UserAccount UserAccount { get; set; }

        public string? PreviousManagerId { get; set; }
        public string NewManagerId { get; set; }

        public UserManagerChangedDomainEvent(UserAccount userAccount, string? previousManagerId, string newManagerId)
        {
            UserAccount = userAccount;
            PreviousManagerId = previousManagerId;
            NewManagerId = newManagerId;
        }
    }
}
