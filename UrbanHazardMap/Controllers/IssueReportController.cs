using Microsoft.AspNetCore.Mvc;

namespace UrbanHazardMap.Controllers
{
    public class IssueReportController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
