using Microsoft.AspNetCore.Mvc;

namespace PayCoTransactions.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
