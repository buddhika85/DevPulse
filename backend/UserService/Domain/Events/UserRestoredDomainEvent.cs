using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserRestoredDomainEvent : INotification
    {
        public UserAccount RestoredUserAccount { get; set; }

        public UserRestoredDomainEvent(UserAccount restoredUserAccount)
        {
            RestoredUserAccount = restoredUserAccount;
        }
    }
}
