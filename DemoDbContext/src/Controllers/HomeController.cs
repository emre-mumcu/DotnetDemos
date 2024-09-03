using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.App_Lib;

namespace src.Controllers;

public class HomeController : Controller
{
	public async Task<IActionResult> Index() => await Task.Run(() => View());

	public IActionResult DbContextInstantiate()
    {
		IConfiguration config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
		
		IServiceProvider sp = HttpContext.RequestServices.GetRequiredService<IServiceProvider>();

		// DbContext instances can be constructed in the normal .NET way, for example with new in C#.
		// Configuration can be performed by overriding the OnConfiguring method, or by passing options to the constructor.
		var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
			.UseSqlite(config.GetConnectionString("DefaultConnection"))
			.EnableDetailedErrors()
			.EnableSensitiveDataLogging()			
			.Options;

		using var context = new AppDbContext(contextOptions, sp);

		return View();
    }
}