using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace DemoAuthenticate.AppLib;

public static class AuthorizationPolicyLibrary
{
	public static AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
	   .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
	   .RequireAuthenticatedUser()
	   .Build();

	public static AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
	   .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
	   .RequireAuthenticatedUser()
	   .Build();

	public static AuthorizationPolicy userPolicy = new AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
		.RequireAuthenticatedUser()		
		.RequireRole("User")
		.Build();

	public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
		.RequireAuthenticatedUser()		
		.AddRequirements(new UserRequirement(new string[] { "Administrator" }))
		// .RequireRole("Administrator")        
		// .RequireAssertion(ctx => { return ctx.User.HasClaim("name1", "val1") || ctx.User.HasClaim("name2", "val2"); })
		.Build();
}