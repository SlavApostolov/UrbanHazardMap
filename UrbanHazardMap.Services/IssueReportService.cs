using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{

    public class IssueReportService:IIssueReportService
    {
        private ApplicationDbContext _context;

        public IssueReportService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return _context.Categories.OrderBy(r=>r.Title).ToList();
        }
        public async Task<int> CreateAsync(string title,string description,double latitude,double longitude,IEnumerable<int> CategoryIds,string? UserId)
        {
            List<Category> categories = new List<Category>();
            foreach (var cID in CategoryIds)
            {
               categories.AddRange(_context.Categories.Where(cat => cat.Id == cID).ToList());
            }
            if (categories.Count == 0) throw new ArgumentException("There are no Ids coresponding to the given categoryIds");
            else
            {
                IssueReport issueReport = new IssueReport();
                issueReport.Title = title;
                issueReport.Description = description;
                issueReport.Latitude = latitude;
                issueReport.Longitude = longitude;
                issueReport.Categories = categories;
                issueReport.ReportedById = UserId;
                return issueReport.Id;
            }
        }
    }
}
