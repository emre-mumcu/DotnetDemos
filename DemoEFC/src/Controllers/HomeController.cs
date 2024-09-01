using Microsoft.AspNetCore.Mvc;

namespace src.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string? view)
        {
            if (string.IsNullOrEmpty(view))
            {
                return View(viewName: "_ContentViewerLayout", model: "about");
            }
            else
                return View(viewName: "_ContentViewerLayout", model: view);
        }
    }
}