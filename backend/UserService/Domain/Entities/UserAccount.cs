using SharedLib.Domain.Entities;
using UserService.Domain.Events;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Entities
{
    public class UserAccount : BaseEntity
    {
        public string Email { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public UserRole Role { get; private set; } = UserRole.User;

        public bool IsDeleted { get; private set; }

        private UserAccount() { }                       // Enforces conrolled instantiation via Create
        

        #region domain_events
        public static UserAccount Create(string email, string displayName, string role, string? objectId = null)
        {
            var user = new UserAccount
            {
                Id = objectId != null ? new Guid(objectId) : Guid.NewGuid(),
                Email = email,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.From(role)
            };
            user.DomainEvents.Add(new UserCreatedDomainEvent(user));
            return user;
        }

        public void UpdateDisplayName(string displayName)
        {
            var oldName = DisplayName;
            DisplayName = displayName;
            DomainEvents.Add(new UserDisplayNameChangedDomainEvent(this, oldName, displayName));
        }

        public void UpdateEmail(string email)
        {
            var oldEmail = Email;
            Email = email;
            DomainEvents.Add(new UserEmailChangedDomainEvent(this, oldEmail, email));
        }


        public void UpdateRole(UserRole role)
        {
            var oldRole = Role;
            Role = role;
            DomainEvents.Add(new UserRoleChangedDomainEvent(this, oldRole, role));
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DomainEvents.Add(new UserSoftDeletedDomainEvent(this));
        }

        public void RestoreUser()
        {
            IsDeleted = false;
            DomainEvents.Add(new UserRestoredDomainEvent(this));
        }

        #endregion domain_events
    }
}
