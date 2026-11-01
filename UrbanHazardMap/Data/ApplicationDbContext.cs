
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Models;

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
        }
    }
}
