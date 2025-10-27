using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserUpdatedDomainEvent : INotification
    {
        public UserAccount UpdatedUserAccount { get; set; }

        public UserUpdatedDomainEvent(UserAccount updatedUserAccount)
        {
            UpdatedUserAccount = updatedUserAccount;
        }
    }
}
