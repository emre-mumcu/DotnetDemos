using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DemoAuthenticate.AppLib;

public class TestAuthorize : IAuthorize
{
	private ClaimsPrincipal GetPrincipal(string UserId) =>
		new ClaimsPrincipal(
			new ClaimsIdentity(
				new List<Claim>() {
						new Claim(ClaimTypes.NameIdentifier, UserId),
						new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
						new Claim(ClaimTypes.Name, "John"),
						new Claim(ClaimTypes.Surname, "Doe"),
						new Claim(ClaimTypes.Email, "john@doe.com"),
						new Claim(ClaimTypes.Role, "Guest"),
						new Claim(ClaimTypes.Role, "User"),
						new Claim(ClaimTypes.Role, "Administrator"),
				},
				CookieAuthenticationDefaults.AuthenticationScheme
			)
		);

	private AuthenticationProperties GetProperties() => new AuthenticationProperties
	{
		AllowRefresh = true,
		ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(Literals.AuthenticationCookie_ExpiresUtc),
		IsPersistent = true,
		IssuedUtc = DateTimeOffset.UtcNow,
		RedirectUri = Literals.AuthenticationCookie_ReturnUrl
	};

	private Task<AuthenticationTicket?> GetAuthenticationTicketAsync(string userId)
	{
		AuthenticationTicket tiket = new AuthenticationTicket(GetPrincipal(userId), GetProperties(), CookieAuthenticationDefaults.AuthenticationScheme);

		TaskCompletionSource<AuthenticationTicket?> tcs = new TaskCompletionSource<AuthenticationTicket?>();

		Task<AuthenticationTicket?> authTask = tcs.Task;

		Task.Factory.StartNew(() =>
		{
			Thread.Sleep(100);

			try
			{
				tcs.SetResult(tiket);
			}
			catch (Exception ex)
			{
				tcs.SetResult(null);
				tcs.SetException(ex);
			}
		});

		return authTask;
	}

	public Task<AuthenticationTicket?> AuthorizeUserAsync(string userId) => GetAuthenticationTicketAsync(userId);

}