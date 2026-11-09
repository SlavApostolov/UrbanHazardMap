using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;
using UrbanHazardMap.ViewModels;
using static System.Formats.Asn1.AsnWriter;


namespace UrbanHazardMap.Controllers
{
    public class IssueReportController : Controller
    {
        private ApplicationDbContext _context;
        public IssueReportController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IActionResult> Create()
        {
            List<Category> categories = _context.Categories.ToList();
            CreateIssueReportViewModel viewModel = new();
            foreach (var category in categories)
            {
                CategoryOptionViewModel temp = new CategoryOptionViewModel();
                temp.Id = category.Id;
                temp.Title=category.Title;
                temp.MarkedHexColor=category.MarkedHexColor;
                viewModel.AvailableCategories.Add(temp);
            }
            return View(viewModel);
        }
    }
}
