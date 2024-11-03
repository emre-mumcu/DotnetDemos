using System.Security.Claims;
using DemoAuthenticate.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DemoAuthenticate.Controllers;

[CommonModel]
public class MyBaseController : Controller
{
		public AuthenticationTicket? GetAuthenticationTicketFromCookie()
	{
		try
		{
			CookieAuthenticationOptions? opt = HttpContext.RequestServices
				.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
				.Get(CookieAuthenticationDefaults.AuthenticationScheme);

			if (opt != null && opt.Cookie.Name != null)
			{
				var authCookie = opt.CookieManager.GetRequestCookie(HttpContext, opt.Cookie.Name);
				AuthenticationTicket? ticket = opt.TicketDataFormat.Unprotect(authCookie);
				return ticket;
			}
			else
			{
				throw new ArgumentException(nameof(CookieAuthenticationOptions));
			}
		}
		catch
		{
			return null;
		}
	}

	public AuthenticationTicket? GetAuthenticationTicketFromAuthResult()
	{
		var authResult = HttpContext.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;
		var authProps = authResult?.Properties;
		return authResult?.Ticket ?? null;
	}

	public Dictionary<string, string>? GetAllCookies(HttpRequest httpRequest)
	{
		var cookies = httpRequest.Cookies;

		var cookieList = new Dictionary<string, string>();

		foreach (var cookie in cookies)
		{
			cookieList[cookie.Key] = cookie.Value;
		}

		return cookieList;
	}

	public async Task<IActionResult> UserLogin(string username)
	{
		var claims = new List<Claim> {
			new Claim(ClaimTypes.NameIdentifier, username),
			new Claim(ClaimTypes.Name, $"Mr. {username}"),
			new Claim(ClaimTypes.Role, "Admin")
		};

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		var principal = new ClaimsPrincipal(identity);

		var authProperties = new AuthenticationProperties
		{
			AllowRefresh = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
			IsPersistent = true,
			IssuedUtc = DateTime.UtcNow,
			RedirectUri = new PathString("/Loggo")
		};

		await HttpContext.SignInAsync(principal, authProperties);


		HttpContext.Session.SetUserSession(Request.HttpContext);

		return RedirectToAction("Index");
	}

	public async Task<IActionResult> UserLogout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		HttpContext.Session.Clear();

		return RedirectToAction("Index");
	}

	public readonly IDataProtector _protectorSessionCookie;

	public readonly IDataProtector _protectorAuthenticationCookie;
	
	public MyBaseController(IDataProtectionProvider dataProtectionProvider)
	{
		// To decrypt authentication cookie:
		// https://github.com/aspnet/Security/blob/rel/1.1.1/src/Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationMiddleware.cs#L40
		// https://stackoverflow.com/questions/42842511/how-to-manually-decrypt-an-asp-net-core-authentication-cookie
		_protectorAuthenticationCookie = dataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", "Cookies", "v2");
		// https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/Session/src/SessionMiddleware.cs#L50
		_protectorSessionCookie = dataProtectionProvider.CreateProtector(nameof(Microsoft.AspNetCore.Session.SessionMiddleware));
	}
}

public class CommonModelAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		Controller? controller = context.Controller as Controller;

		if (controller != null)
		{
			controller.ViewBag.Current = new CommonVM()
			{				
				Ticks = DateTime.UtcNow.Ticks,
				MilliSecs = (DateTime.UtcNow.Ticks / 10000) - 62135596800000				
			};

			if (context.HttpContext.Session.IsAvailable && context.HttpContext.Session.Get(Literals.SessionCookieName) != null)
			{
				controller.ViewBag.Session = new CommonVM()
				{
					Ticks = context.HttpContext.Session.Get<DateTime>(Literals.SessionCookieName).Ticks,
					MilliSecs = (context.HttpContext.Session.Get<DateTime>(Literals.SessionCookieName).Ticks / 10000) - 62135596800000
				};
			}
		}

		base.OnActionExecuting(context);
	}
}

/*
	The .NET DateTime type's origin is midnight on 1 January 0001.
	The JavaScript Date type's origin is the Unix epoch, midnight on 1 January 1970.
	The number of .net ticks at the unix epoch is 621355968000000000.
	There are 10000 ticks in a millisecond. 
	And there are 621.355.968.000.000.000 ticks between 1st Jan 0001 and 1st Jan 1970.

	var ticks = ((yourDateObject.getTime() * 10000) + 621355968000000000);
*/