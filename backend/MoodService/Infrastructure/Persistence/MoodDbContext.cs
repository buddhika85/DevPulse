using Microsoft.EntityFrameworkCore;
using MoodService.Domain.Entities;
using MoodService.Domain.ValueObjects;

namespace MoodService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\MoodService>
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    public class MoodDbContext : DbContext
    {
        public MoodDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureColumns(modelBuilder);
            ConfgureValueObjectToStringConversions(modelBuilder);
            ApplyIndexes(modelBuilder);
        }

        private void ConfigureColumns(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoodEntry>()
               .Property(t => t.UserId)
               .IsRequired();

            modelBuilder.Entity<MoodEntry>()
               .Property(t => t.Day)
               .IsRequired();

            modelBuilder.Entity<MoodEntry>()
               .Property(t => t.MoodTime)
               .IsRequired();

            modelBuilder.Entity<MoodEntry>()
               .Property(t => t.MoodLevel)
               .IsRequired();

            modelBuilder.Entity<MoodEntry>()
              .Property(t => t.CreatedAt)
              .IsRequired();

            modelBuilder.Entity<MoodEntry>()
               .Property(t => t.Note)
               .HasColumnType("nvarchar(max)");
        }

        private void ConfgureValueObjectToStringConversions(ModelBuilder modelBuilder)
        {
            // Configure value conversion for MoodTime value object (stored as string)
            modelBuilder.Entity<MoodEntry>()
                .Property(t => t.MoodTime)
                .IsRequired()
                .HasColumnName("MoodTime")      // Column name in DB
                .HasMaxLength(50)
                .HasConversion(
                    v => v.Value,                    // To string for DB
                    v => MoodTime.From(v)); // Back to Value Object

            // Configure value conversion for MoodLevel value object (stored as string)
            modelBuilder.Entity<MoodEntry>()
                .Property(t => t.MoodLevel)
                .IsRequired()
                .HasColumnName("MoodLevel")      // Column name in DB
                .HasMaxLength(50)
                .HasConversion(
                    v => v.Value,                    // To string for DB
                    v => MoodLevel.From(v)); // Back to Value Object
        }

        private void ApplyIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoodEntry>()
               .HasIndex(t => t.UserId)
               .HasDatabaseName("IX_MoodEntry_UserId"); // For filtering mood by user

            modelBuilder.Entity<MoodEntry>()
               .HasIndex(t => t.Day)
               .HasDatabaseName("IX_MoodEntry_Day"); // For filtering mood by day

            modelBuilder.Entity<MoodEntry>()
               .HasIndex(t => t.MoodTime)
               .HasDatabaseName("IX_MoodEntry_Time"); // For filtering mood by time

            // One mood entry per user per day per session
            modelBuilder.Entity<MoodEntry>()
                .HasIndex(t => new { t.UserId, t.Day, t.MoodTime })
                .IsUnique()
                .HasDatabaseName("UX_MoodEntry_User_Day_Time");
        }
    }
}
