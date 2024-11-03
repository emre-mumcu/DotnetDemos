using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DemoAuthenticate.AppLib;

public class UserRequirement : IAuthorizationRequirement
{
	public string[] Roles { get; private set; }

	public UserRequirement(params string[] roles)
	{
		Roles = roles;
	}
}

public class UserHandler : AuthorizationHandler<UserRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
	{
		try
		{
			string[] userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

			if (requirement.Roles.Intersect(userRoles).Any())
			{
				context.Succeed(requirement);
			}
			else
			{
				context.Fail();
			}

			return Task.CompletedTask;
		}
		catch
		{
			throw;
		}
	}
}

/*
To prevent HandleRequirementAsync method executing twice:
---------------------------------------------------------

Link: https://github.com/dotnet/aspnetcore/issues/32518

Starting in 3.1, authorization attributes are handled by the authorization middleware instead of being evaluated by MVC's authorization filter. 
However if you explicitly add an AuthorizationFilter, it would get separately executed.

Instead of adding a AuthorizationFilter, consider using using RequireAuthorization on an endpoint: e.g.

endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization("some_policy");

This would a substitute to adding a global auth filter. 
 */