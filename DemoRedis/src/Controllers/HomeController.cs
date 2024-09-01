using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using src.App_Lib;
using src.App_Lib.RedisStack;
using StackExchange.Redis;

namespace src.Controllers;

public class HomeController : Controller
{
	// public async Task<IActionResult> Index() => await Task.Run(() => View());
	public async Task<IActionResult> Index()
	{
		HttpContext.Session.SetString("key-1", "value-1");
		return await Task.Run(() => View());
	}

	// Request IConnectionMultiplexer from IoC
	// Requires the following service registration:
	// builder.Services.AddSingleton<IConnectionMultiplexer>(...);
	public async Task<IActionResult> Redis1([FromServices] IConnectionMultiplexer connectionMultiplexer)
	{
		var db = connectionMultiplexer.GetDatabase();

		db.StringSet("Redis1", Guid.NewGuid().ToString());

		TempData["Message"] = "Redis1 task is completed!!!";

		return await Task.Run(() => View(viewName: "Index"));
	}

	public async Task<IActionResult> Redis2()
	{
		var db = RedisConnector.Connection.GetDatabase();

		db.StringSet("Redis2", Guid.NewGuid().ToString());

		TempData["Message"] = "Redis2 task is completed!!!";

		return await Task.Run(() => View(viewName: "Index"));
	}

	// IDistributedCache is SET by builder.Services.AddStackExchangeRedisCache
	public async Task<IActionResult> DistributedCache([FromServices] IDistributedCache cache)
	{
		var id = "1";

		var cacheKey = $"item:{id}";

		var cachedStr = await cache.GetOrSetAsync(cacheKey,
			async () =>
			{
				// cache is missing the requested item.
				// get item from data store and add to cache				
				return await Task.Run(() => "Sample Data");
			})!;

		TempData["Message"] = "DistributedCache task is completed!!!";

		return await Task.Run(() => View(viewName: "Index"));
	}
}