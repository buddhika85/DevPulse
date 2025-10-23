using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
//using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\UserService>
    // dotnet ef migrations add InitialUserMigration 
    // dotnet ef database update
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           

            modelBuilder.Entity<UserAccount>(builder =>
            {
                builder.OwnsOne(entity => entity.Role, roleBuilder =>
                {
                    roleBuilder.Property(role => role.Value)
                    .IsRequired()
                    .HasColumnName("Role")
                    .HasMaxLength(50);
                });
            });
        }
    }
}
