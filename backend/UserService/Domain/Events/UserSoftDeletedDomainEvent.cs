using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserSoftDeletedDomainEvent : INotification
    {
        public UserAccount UserAccountDeleted { get; set; }

        public UserSoftDeletedDomainEvent(UserAccount userAccountDeleted)
        {
            UserAccountDeleted = userAccountDeleted;
        }
    }
}
