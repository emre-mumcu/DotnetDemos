using System;

namespace DemoAuthenticate.AppLib;

public class TestAuthenticate : IAuthenticate
{
	public bool AuthenticateUser(string UserId, string Password) => true;
}
