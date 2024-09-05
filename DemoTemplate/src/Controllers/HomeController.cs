using Microsoft.AspNetCore.Mvc;

namespace src.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index() => await Task.Run(() => View());
}
