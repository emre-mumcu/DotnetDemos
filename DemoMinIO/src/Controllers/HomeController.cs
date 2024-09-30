using Microsoft.AspNetCore.Mvc;

namespace src.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    public IActionResult Index() => View();

}