using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using src.Models;

namespace src.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogTrace($"LogTrace {LogLevel.Trace}"); // 0
        _logger.LogDebug($"LogDebug {LogLevel.Debug}"); // 1
        _logger.LogInformation($"LogInformation {LogLevel.Information}"); // 2
        _logger.LogWarning($"LogWarning {LogLevel.Warning}"); // 3
        _logger.LogError($"LogError {LogLevel.Error}"); // 4
        _logger.LogCritical ($"LogCritical {LogLevel.Critical}"); // 5
        _logger.Log(LogLevel.None, "NONE"); // 6        

        return View();
    }

    
}
