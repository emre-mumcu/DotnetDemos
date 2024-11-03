using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace DemoAuthenticate.AppLib;

public static class HttpContextExtensions
{
	public static AuthenticationTicket? GetAuthenticationTicketFromCookie(this HttpContext httpContext)
	{
		try
		{
			CookieAuthenticationOptions? opt = httpContext.RequestServices
				.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
				.Get(CookieAuthenticationDefaults.AuthenticationScheme);

			if (opt != null && opt.Cookie.Name != null)
			{
				var authCookie = opt.CookieManager.GetRequestCookie(httpContext, opt.Cookie.Name);
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

	public static AuthenticationTicket? GetAuthenticationTicketFromAuthResult(this HttpContext httpContext)
	{
		var authResult = httpContext.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;
		var authProps = authResult?.Properties;
		return authResult?.Ticket ?? null;
	}

	public static Dictionary<string, string>? GetAllCookies(this HttpContext httpContext)
	{
		var cookies = httpContext.Request.Cookies;

		var cookieList = new Dictionary<string, string>();

		foreach (var cookie in cookies)
		{
			cookieList[cookie.Key] = cookie.Value;
		}

		return cookieList;
	}

}