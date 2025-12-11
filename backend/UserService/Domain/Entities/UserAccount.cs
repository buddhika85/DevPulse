using SharedLib.Domain.Entities;
using SharedLib.Domain.ValueObjects;
using UserService.Domain.Events;

namespace UserService.Domain.Entities
{
    public class UserAccount : BaseEntity
    {
        public string Email { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public UserRole Role { get; private set; } = UserRole.User;

        public bool IsDeleted { get; private set; }


       
        public Guid? ManagerId { get; private set; }

        // navigational property
        public virtual UserAccount? Manager { get; private set; }

        // manager can have many direct reports
        public virtual ICollection<UserAccount> DirectReports { get; private set; } = [];

        // 1 : M relationships
        // User has 0 to many TaskItems
        // User has 0 to many Moods
        // User has 0 to many JorunalEntries
        // Manager User has 0 to many JounalFeedbacks
        // we dont keep any navigational properties in entities
        // as the reference DB tables live in other micro service specific DBs
        // - EF do not have access to them

        private UserAccount() { }                       // Enforces conrolled instantiation via Create
        

        #region domain_events
        public static UserAccount Create(string email, string displayName, string role, string? objectId = null)
        {
            var user = new UserAccount
            {
                Id = objectId != null ? new Guid(objectId) : new Guid(),        // Entra object Id - coming from Microsof entra external ID
                Email = email,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.From(role),
                ManagerId = null                                                    // on user creation manager ID is null
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

        public void UpdateManager(Guid? managerId)
        {
            var oldManagerId = ManagerId;    
            ManagerId = managerId;
            DomainEvents.Add(new UserManagerChangedDomainEvent(this, oldManagerId?.ToString(), managerId?.ToString()));
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
