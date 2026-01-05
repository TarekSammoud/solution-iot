using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    public class AlerteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
