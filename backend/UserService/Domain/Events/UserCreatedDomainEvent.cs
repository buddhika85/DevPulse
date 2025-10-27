using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    // This event is a business signal: “An user account was created.”
    // - It’s just a pure domain event, ready to be handled by anyone/any code that cares.
    public class UserCreatedDomainEvent : INotification
    {
        public UserAccount UserAccountCreated { get; set; }

        public UserCreatedDomainEvent(UserAccount userAccountCreated)
        {
            UserAccountCreated = userAccountCreated;
        }
    }
}
