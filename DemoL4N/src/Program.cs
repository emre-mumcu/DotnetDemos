// dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
// dotnet add package log4net
// add log4net.config file
// add Log4netExtensions class
// log4net xsd schema: https://csharptest.net/downloads/schema/log4net.xsd

using src.AppLib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLog4net();

builder.Services.AddMvc().AddRazorRuntimeCompilation();

var app = builder.Build();

app.MapDefaultControllerRoute();

app.Run();


// Alternate Use:
// --------------
// You can integrate log4net with the Microsoft.Extensions.Logging framework. This allows you to use log4net as the underlying logging provider while taking advantage of the built-in logging abstractions provided by .NET.

// dotnet add package Microsoft.Extensions.Logging.Log4Net.AspNetCore

// builder.Logging.ClearProviders();
// builder.Logging.AddLog4Net("log4net.config");

// app.MapGet("/", (ILogger<Program> logger) =>
// {
//     logger.LogDebug("Hello, log4net with Microsoft.Extensions.Logging!");
//     return "Hello, log4net with minimal API!";
// });

// public class SampleController : ControllerBase
// {    // Get logger either from LogManager
//      private readonly ILog _logger = LogManager.GetLogger(typeof(SampleController));

        // Or from DI
//     private readonly ILogger<SampleController> _logger;
//     public SampleController(ILogger<SampleController> logger)
//     {
//         _logger = logger;
//     }
// }