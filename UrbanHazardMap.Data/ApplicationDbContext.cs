
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IssueReport> IssueReports { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Institution> Institutions { get; set; }

        public DbSet<VolunteerEvent> VolunteerEvents { get; set; }

        public DbSet<ReportConfirmation> ReportConfirmations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Institution)
                .WithOne(i => i.Category)
                .HasForeignKey<Category>(c => c.InstitutionId);

            modelBuilder.Entity<IssueReport>()
                .HasMany(i => i.Categories)
                .WithMany(c => c.IssueReports);

            modelBuilder.Entity<IssueReport>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<IssueReport>()
                .HasOne(i => i.ReportedByUser)
                .WithMany()
                .HasForeignKey(i => i.ReportedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ReportConfirmation>()
                .HasIndex(c => new { c.IssueReportId, c.UserId })
                .IsUnique();

            // When a user is deleted (GDPR, Task 37), their confirmations are deleted too.
            modelBuilder.Entity<ReportConfirmation>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReportConfirmation>()
                .HasOne(c => c.IssueReport)
                .WithMany(r => r.Confirmations)
                .HasForeignKey(c => c.IssueReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
