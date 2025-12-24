using JournalService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JournalService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\JournalService>
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    public class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions options) : base(options)
        {
        }


        // 1:1 Jounrnal can have exatly 1 Feedback, while Feedback can have exactly 1 JournalEntry
        // Jounral: Entry
        // JounralFeedback: Child
        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
        public DbSet<JournalFeedback> JournalFeedbacks => Set<JournalFeedback>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureColumns(modelBuilder);            
            ApplyIndexes(modelBuilder);
        }

        #region columns_config

        private void ConfigureColumns(ModelBuilder modelBuilder)
        {
            ConfigureRelationships(modelBuilder);
            ConfigureJournalEntryColumns(modelBuilder);
            ConfigureJournalFeedbackColumns(modelBuilder);
        }


        // 1:1 Jounrnal can have exatly 1 Feedback, while Feedback can have exactly 1 JournalEntry
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalEntry>()
                .HasOne(j => j.JournalFeedback)
                .WithOne(f => f.JournalEntry)
                .HasForeignKey<JournalFeedback>(f => f.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);                          // - Cascade delete ensures feedback is removed when journal is deleted
        }

        private void ConfigureJournalEntryColumns(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalEntry>()
                           .Property(t => t.Title)
                           .HasColumnType("nvarchar(500)")
                           .IsRequired();

            modelBuilder.Entity<JournalEntry>()
              .Property(t => t.Content)
              .HasColumnType("nvarchar(max)")
              .IsRequired();  

            modelBuilder.Entity<JournalEntry>()
               .Property(t => t.UserId)
               .IsRequired();

            modelBuilder.Entity<JournalEntry>()
               .Property(t => t.CreatedAt)
               .IsRequired();

            modelBuilder.Entity<JournalEntry>()
               .Property(t => t.IsDeleted)
               .IsRequired();
        }

        private void ConfigureJournalFeedbackColumns(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalFeedback>()
                           .Property(t => t.Comment)
                           .HasColumnType("nvarchar(max)")
                           .IsRequired();

            modelBuilder.Entity<JournalFeedback>()
                           .Property(t => t.JournalEntryId)
                           .IsRequired();

            modelBuilder.Entity<JournalFeedback>()
                          .Property(t => t.FeedbackManagerId)
                          .IsRequired();
        }

        #endregion columns_config

        private void ApplyIndexes(ModelBuilder modelBuilder)
        {
            ApplyJournalEntryIndexes(modelBuilder);
            ApplyJournalFeedbackIndexes(modelBuilder);
        }
        
        private void ApplyJournalEntryIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalEntry>()
               .HasIndex(t => t.UserId)
               .HasDatabaseName("IX_JournalEntry_UserId"); // For filtering Journal by user

            modelBuilder.Entity<JournalEntry>()
               .HasIndex(t => t.Title)
               .HasDatabaseName("IX_JournalEntry_Title"); // For filtering Journal by title          
        }

        private void ApplyJournalFeedbackIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalFeedback>()
              .HasIndex(t => t.FeedbackManagerId)
              .HasDatabaseName("IX_JournalEntry_FeedbackManagerId"); // For filtering Journal by FeedbackManager

            modelBuilder.Entity<JournalFeedback>()                      // Jounrnal : JounrnalFeedback 1:1, enforce it at DB level
                .HasIndex(f => f.JournalEntryId)
                .IsUnique();
        }
    }
}
