using MediatR;
using UserService.Domain.Entities;

namespace UserService.Domain.Events
{
    public class UserEmailChangedDomainEvent : INotification
    {
        public UserAccount UserAccount { get; set; }
        public string PreviousEmail { get; set; }
        public string NewEmail { get; set; }

        public UserEmailChangedDomainEvent(UserAccount userAccount, string previousEmail, string newEmail)
        {
            UserAccount = userAccount;
            PreviousEmail = previousEmail;
            NewEmail = newEmail;
        }
    }
}
