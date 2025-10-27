using MediatR;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Events
{
    public class UserRoleChangedDomainEvent : INotification
    {
        public UserAccount UserAccount { get; set; }
        public UserRole PreviousRole { get; set; }
        public UserRole NewRole { get; set; }

        public UserRoleChangedDomainEvent(UserAccount userAccount, UserRole previousRole, UserRole newRole)
        {
            UserAccount = userAccount;
            PreviousRole = previousRole;
            NewRole = newRole;
        }
    }
}
