using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CallCenter.Data;

namespace CallCenter_WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataBaseContext _dbContext;
        public HomeController(DataBaseContext c)
        {
            _dbContext = c;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
        
        public IActionResult CreateTestData()
        {
            DataHelper.AddTestData(_dbContext);
            return Redirect("Index");
        }
    }
}
