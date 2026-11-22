using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanHazardMap.Data.Models
{
    public class ReportConfirmation
    {
        public int Id { get; set; }
        public int IssueReportId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IssueReport? IssueReport { get; set; }
        public IdentityUser? User { get; set; }
    }
}
