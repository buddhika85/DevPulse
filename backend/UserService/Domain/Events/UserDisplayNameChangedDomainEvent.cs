using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserDisplayNameChangedDomainEvent : INotification
    {
        public UserAccount UpdatedAccount { get; }
        public string OldDisplayName { get; }
        public string NewDisplayName { get; }

        public UserDisplayNameChangedDomainEvent(UserAccount updatedAccount, string oldDisplayName, string newDisplayName)
        {
            UpdatedAccount = updatedAccount;
            OldDisplayName = oldDisplayName;
            NewDisplayName = newDisplayName;
        }
    }
}
