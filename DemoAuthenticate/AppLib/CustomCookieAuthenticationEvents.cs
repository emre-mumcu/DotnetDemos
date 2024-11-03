using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DemoAuthenticate.AppLib;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
	public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
	{
		// Runs in every request
		Boolean login = context.HttpContext.Session.Get<bool>(Literals.SessionKeyLogin);

		if (context.Principal != null && context.Principal.Identity != null)
		{
			if (!(context.Principal.Identity.IsAuthenticated && login))
			{
				// Bu şekilde kapanmış tarayıcıdan gelenler
				// auth cookieleri olsa bile sessionları değiştiği için
				// yeniden login olmadan işlem yapamazlar!!!
				context.RejectPrincipal();
				await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}
		}
		else
		{
			// TODO: What to do?
		}
	}
}
