using System;

namespace DemoAuthenticate.AppLib;

public static class Literals
{
	///
	/// Application Keys
	///

	// Session Cookie
	public static string SessionCookie_Name = "OturumKurabiyesi";
	public static double SessionCookie_IdleTimeout = 30;

	// Authentication Cookie
	public static PathString AuthenticationCookie_Path = new PathString("/");
	public static double AuthenticationCookie_ExpiresUtc = SessionCookie_IdleTimeout;
	public static string AuthenticationCookie_Name = "KimlikDogrulamaKurabiyesi";
	public static PathString AuthenticationCookie_LoginPath = new PathString("/Login");
	public static PathString AuthenticationCookie_LogoutPath = new PathString("/Logout");
	public static PathString AuthenticationCookie_AccessDeniedPath = new PathString("/AccessDenied");
	public static PathString AuthenticationCookie_ReturnUrl = new PathString("/ReturnUrl");

	///
	/// Session Keys
	///

	public static string SessionKey_Created = nameof(SessionKey_Created);
	public static string SessionKey_Login = nameof(SessionKey_Login);
	public static string SessionKey_User = nameof(SessionKey_User);
	public static string SessionKey_LastRequest = nameof(SessionKey_LastRequest);
}
