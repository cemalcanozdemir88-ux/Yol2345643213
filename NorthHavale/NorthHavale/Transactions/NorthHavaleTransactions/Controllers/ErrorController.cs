using Microsoft.AspNetCore.Mvc;

namespace NorthHavaleTransactions.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotingFound()
        {
            return View();
        }
    }
}
