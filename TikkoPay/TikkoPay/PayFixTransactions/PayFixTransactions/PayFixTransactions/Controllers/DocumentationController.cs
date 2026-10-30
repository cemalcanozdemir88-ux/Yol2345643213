using Microsoft.AspNetCore.Mvc;

namespace PayFixTransactions.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
