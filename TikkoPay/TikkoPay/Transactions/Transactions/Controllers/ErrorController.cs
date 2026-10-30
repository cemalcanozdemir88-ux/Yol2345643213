using Microsoft.AspNetCore.Mvc;

namespace Transactions.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotingFound()
        {
            return View();
        }
    }
}
