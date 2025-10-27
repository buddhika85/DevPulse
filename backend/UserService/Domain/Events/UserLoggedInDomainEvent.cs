using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserLoggedInDomainEvent : INotification
    {
        public UserAccount UserAccount { get; set; }
        public DateTime LoginTime { get; set; }

        public UserLoggedInDomainEvent(UserAccount userAccount, DateTime loginTime)
        {
            UserAccount = userAccount;
            LoginTime = loginTime;
        }
    }
}
