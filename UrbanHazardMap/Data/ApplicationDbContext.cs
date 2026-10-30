using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Models;

namespace UrbanHazardMap.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IssueReport> IssueReports { get; set; }
        public DbSet<Category> Categorys { get; set; }
    }
}
