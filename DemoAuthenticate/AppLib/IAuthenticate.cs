using System;

namespace DemoAuthenticate.AppLib;

public interface IAuthenticate
{
	public Task<bool> AuthenticateUserAsync(string UserId, string Password);
}
