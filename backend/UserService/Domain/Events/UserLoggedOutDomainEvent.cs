using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserLoggedOutDomainEvent : INotification
    {
        public UserLoggedOutDomainEvent(UserAccount userAccount, DateTime loggedOutTime)
        {
            UserAccount = userAccount;
            LoggedOutTime = loggedOutTime;
        }

        public UserAccount UserAccount { get; set; }
        public DateTime LoggedOutTime { get; set; }
    }
}
