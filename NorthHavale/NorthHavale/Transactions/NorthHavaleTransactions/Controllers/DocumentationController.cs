using Microsoft.AspNetCore.Mvc;

namespace NorthHavaleTransactions.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult v2()
        {
            return View();
        }
    }
}
