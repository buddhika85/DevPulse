using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence
{
    public static class DbInitializer
    {

        public static void Seed(UserDbContext context)
        {
            if (context.UserAccounts.Any()) return; // Already seeded

            var users = new[]
            {
                UserAccount.Create("admin@devpulse.local", "Admin User", "admin"),                
                UserAccount.Create("manager@devpulse.local", "Manager", "manager"),
                UserAccount.Create("user@devpulse.local", "User", "user")
            } ;

            context.UserAccounts.AddRange(users);
            context.SaveChanges();
        }

    }
}
