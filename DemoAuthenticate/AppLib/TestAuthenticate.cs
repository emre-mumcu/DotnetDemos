using System;

namespace DemoAuthenticate.AppLib;

public class TestAuthenticate : IAuthenticate
{
	public Task<bool> AuthenticateUserAsync(string UserId, string Password) => Task.FromResult<bool>(true);
}
