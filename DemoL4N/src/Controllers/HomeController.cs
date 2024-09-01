using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace src.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILog _logger;

        public HomeController(ILog logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        string ReadAllText(string file)
        {
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var textReader = new StreamReader(fileStream);
            return textReader.ReadToEnd();
        }

        public IActionResult Index([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            _logger.Debug("Debug");
            _logger.Info("Info");
            _logger.Warn("Warn");
            _logger.Error("Error", new Exception("This is ex"));
            _logger.Fatal("Fatal");

            var p1 = Directory.GetCurrentDirectory();
            var p2 = webHostEnvironment.ContentRootPath;
            var p3 = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            string rollingFileName1 = $"AppAllLog_{DateTime.Now.ToString("yyyyMMdd")}.log";
            string rollingFileName2 = $"AppErrLog_{DateTime.Now.ToString("yyyyMMdd")}.log";

            string logPath1 = Path.Combine(p3 ?? "", rollingFileName1);
            string logPath2 = Path.Combine(p3 ?? "", rollingFileName2);

            string contents1 = ReadAllText(logPath1);
            string contents2 = ReadAllText(logPath2);

            HomeVM homeVM = new HomeVM() { AllLogs = contents1, ErrorLogs = contents2 };

            return View(model: homeVM);
        }
    }

    public class HomeVM
    {
        public string? AllLogs { get; set; }
        public string? ErrorLogs { get; set; }
    }

}
