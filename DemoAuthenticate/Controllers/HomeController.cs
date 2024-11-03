using DemoAuthenticate.AppLib;
using DemoAuthenticate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DemoAuthenticate.Controllers;

[AllowAnonymous]
public class HomeController : MyBaseController
{
	public HomeController(IDataProtectionProvider dataProtectionProvider): base(dataProtectionProvider) { }
	
	public ActionResult Index()
	{		
		IndexVM model = new IndexVM()
		{
			SessionFeature = HttpContext.Features.Get<ISessionFeature>(),
			SessionOptions = HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<SessionOptions>>().CurrentValue,
			RequestCookieCollection = HttpContext.Request.Cookies,
			RequestCookieDictionary = HttpContext.Request.Cookies.ToDictionary(c => c.Key, c => c.Value),
			SessionKeysDictionary = HttpContext.Session.Keys.ToDictionary(c => HttpContext.Session.GetString(c)!),			
			AllCookies = HttpContext.GetAllCookies(),
			SessionCookie = HttpContext.Request.Cookies[Literals.SessionCookie_Name],
			AuthCookie = HttpContext.Request.Cookies[Literals.AuthenticationCookie_Name],
			AuthenticationTicketFromCookie = HttpContext.GetAuthenticationTicketFromCookie(),
			AuthenticationTicketFromAuthResult = HttpContext.GetAuthenticationTicketFromAuthResult()
		};

		if (Request.Cookies.TryGetValue(Literals.SessionCookie_Name, out var sessionCookie))
		{
			// Manually decrypting a cookie is virtually useless
			model.DecryptedSessionCookie = _protectorSessionCookie.Unprotect(sessionCookie);			
		}

		if (Request.Cookies.TryGetValue(Literals.AuthenticationCookie_Name, out var authCookie))
		{
			// Manually decrypting a cookie is virtually useless
			model.DecryptedAuthCookie = _protectorAuthenticationCookie.Unprotect(authCookie);
		}

		return View(model);
	}

	[HttpGet("/Admin")]
	[Authorize(Policy = "AdminPolicy")]
	public IActionResult Admin() => View();

	[HttpGet("/Login")]
	public async Task<IActionResult> Login() => await UserLogin("DemoUser");

	[HttpGet("/AccessDenied")]
	public IActionResult AccessDenied() => View();
	
	[HttpGet("/Logout")]
	public async Task<IActionResult> Logout() => await UserLogout();
}