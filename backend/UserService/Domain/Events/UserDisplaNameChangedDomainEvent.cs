using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserDisplaNameChangedDomainEvent : INotification
    {
        public UserAccount UpdatedAccount { get; }
        public string OldDisplayName { get; }
        public string NewDisplayName { get; }

        public UserDisplaNameChangedDomainEvent(UserAccount updatedAccount, string oldDisplayName, string newDisplayName)
        {
            UpdatedAccount = updatedAccount;
            OldDisplayName = oldDisplayName;
            NewDisplayName = newDisplayName;
        }
    }
}
