using SharedLib.Domain.Entities;
using UserService.Domain.Events;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Entities
{
    public class UserAccount : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; set; } = UserRole.User;



        private UserAccount() { }                       // Enforces conrolled instantiation via Create
        

        #region domain_events
        public static UserAccount Create(string email, string displayName, string role)
        {
            var user = new UserAccount
            {
                Email = email,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.From(role)
            };
            user.DomainEvents.Add(new UserCreatedDomainEvent(user));
            return user;
        }


        #endregion domain_events
    }
}
