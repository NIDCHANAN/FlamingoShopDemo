using Microsoft.AspNetCore.Mvc;

namespace FlamingoShopDemo.Controllers.Admin
{
    public class MonitorController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.role = "admin";

            return View();
        }
    }
}
