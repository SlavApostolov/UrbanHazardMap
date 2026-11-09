using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;
using UrbanHazardMap.ViewModels;
using UrbanHazardMap.Services;
using static System.Formats.Asn1.AsnWriter;
using AspNetCoreGeneratedDocument;


namespace UrbanHazardMap.Controllers
{
    public class IssueReportController : Controller

    {
        private IIssueReportService _issueReportService;
        public IssueReportController(IIssueReportService issueReportService) 
        {
            _issueReportService = issueReportService;
        }
        public async Task<IActionResult> Create()
        {
            List<Category> categories =  await _issueReportService.GetAllCategoriesAsync();
            CreateIssueReportViewModel viewModel = new();
            foreach (var category in categories)
            {
                CategoryOptionViewModel temp = new CategoryOptionViewModel();
                temp.Id = category.Id;
                temp.Title = category.Title;
                temp.MarkedHexColor = category.MarkedHexColor;
                viewModel.AvailableCategories.Add(temp);
            }
            return View(viewModel);
        }
    }
}
