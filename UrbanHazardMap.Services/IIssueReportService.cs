using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public interface IIssueReportService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<int> CreateAsync(string title, string description, double latitude, double longitude, IEnumerable<int> categoryIds, string? userId);
    }
}
