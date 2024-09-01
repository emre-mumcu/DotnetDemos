using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace src.Lib
{
    public class HomeController : Controller
    {
        public IActionResult Index(
            [FromServices] IConfiguration configuration,
            [FromServices] IOptions<DatabaseOptions> staticOptions, // reloadOnChange true bile olsa değerler güncellenmez
            [FromServices] IOptionsMonitor<DatabaseOptions> dynamicOptions
            )
        {
            
            var obj1 = configuration.GetSection("Database").Get<DatabaseOptions>();
            var obj2 = staticOptions.Value;
            var obj3 = dynamicOptions.CurrentValue;
            var cs = configuration["Database:ConnectionString"];

            return Content($@"
                IConfiguration GetSection   : {obj1!.ConnectionString}
                IOptions                    : {obj2.ConnectionString}
                IOptionsMonitor             : {obj3.ConnectionString}
                IConfiguration              : {cs}
            ");
        }
    }
}