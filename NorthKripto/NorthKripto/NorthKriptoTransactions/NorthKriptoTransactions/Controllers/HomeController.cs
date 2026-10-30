using Microsoft.AspNetCore.Mvc;
using NorthKriptoTransactions.Models;

namespace NorthKriptoTransactions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetWallet([FromBody] GetWalletRequest request)
        {
            return Json(new { message = "", status = true});
        }
    }
}
