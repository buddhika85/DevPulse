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
                // Index on Email for faster lookup
                builder.HasIndex(x => x.Email)
                       .IsUnique()
                       .HasDatabaseName("IX_UserAccount_Email");

                builder.HasIndex(x=> x.IsDeleted)
                        .HasDatabaseName("IX_UserAccount_IsDeleted");


                // mapping user role value object to be stored as a column which stores string value of user role
                builder.OwnsOne(entity => entity.Role, roleBuilder =>
                {
                    roleBuilder.Property(role => role.Value)
                    .IsRequired()
                    .HasColumnName("Role")
                    .HasMaxLength(50);

                    // Composite index on Role and IsDeleted
                    roleBuilder.HasIndex(role => role.Value)
                            .HasDatabaseName("IX_UserAccount_Role");

                });
            });
        }
    }
}
