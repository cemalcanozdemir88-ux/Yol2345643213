using Microsoft.AspNetCore.Mvc;

namespace Transactions.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
