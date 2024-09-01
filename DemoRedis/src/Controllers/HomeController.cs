using Microsoft.AspNetCore.Mvc;
using src.App_Lib.RedisStack;

namespace src.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
		var db = RedisConnector.Connection.GetDatabase();

		db.StringSet("Index", Guid.NewGuid().ToString());

		return Ok();
    }
}