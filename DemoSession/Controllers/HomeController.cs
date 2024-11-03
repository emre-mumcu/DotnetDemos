using DemoSession.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace DemoSession.Controllers
{
    public class HomeController : Controller
    {
		private readonly IDataProtector _protector;

		public HomeController(IDataProtectionProvider dataProtectionProvider)
		{
			//
			// https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/Session/src/SessionMiddleware.cs
			//
			
			// _protector = dataProtectionProvider.CreateProtector("SessionMiddleware");
			_protector = dataProtectionProvider.CreateProtector(nameof(Microsoft.AspNetCore.Session.SessionMiddleware));
        }
		
		public ActionResult Index()
        {
			HomeVM model = new HomeVM() 
			{
				SessionFeature = HttpContext.Features.Get<ISessionFeature>(),
				RequestCookieCollection = HttpContext.Request.Cookies,
				RequestCookieDictionary = HttpContext.Request.Cookies.ToDictionary(c => c.Key, c => c.Value),
				SessionKeysDictionary = HttpContext.Session.Keys.ToDictionary(c => HttpContext.Session.GetString(c)!),
				SessionCookie = HttpContext.Request.Cookies[Literals.Session_CookieName],				
			};

			if (Request.Cookies.TryGetValue(Literals.Session_CookieName, out var sessionCookie)) model.DecryptedSessionCookie = _protector.Unprotect(sessionCookie);

			//var sessionData = new Dictionary<string, string>(); foreach (var key in HttpContext.Session.Keys) sessionData[key] = HttpContext.Session.GetString(key);
			
			return View(model);
        }

		public IActionResult AddSessionData()
		{
			HttpContext.Session.Set<DateTime>("SessionData", DateTime.UtcNow);

			return RedirectToAction("Index");
		}

		public IActionResult DeleteCookies()
		{
			foreach (var cookie in Request.Cookies.Keys)
			{
				Response.Cookies.Delete(cookie);
			}

			return RedirectToAction("Index");
		}
    }
}