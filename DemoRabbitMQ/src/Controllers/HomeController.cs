using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using src.App_Lib;

namespace src.Controllers;

public class HomeController(ILogger<HomeController> _logger, RabbitMqSenderService rabbitMqService) : Controller
{
    public IActionResult Index()
    {
		return View();
    }


	[HttpPost]
	public IActionResult SendMessage(string message)
	{
		rabbitMqService.SendMessage("test_queue", message);
		return RedirectToAction("Index");
	}
}
