using System;
using Microsoft.AspNetCore.Authentication;

namespace DemoAuthenticate.AppLib;

public interface IAuthorize
{
	public Task<AuthenticationTicket?> AuthorizeUserAsync(string UserId);
}
