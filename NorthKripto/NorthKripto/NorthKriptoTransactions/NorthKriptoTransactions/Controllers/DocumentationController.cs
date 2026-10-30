using Microsoft.AspNetCore.Mvc;

namespace NorthKriptoTransactions.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
