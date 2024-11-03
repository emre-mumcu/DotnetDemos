using System;
using Microsoft.AspNetCore.Http.Features;

namespace DemoSession.ViewModels;

public class HomeVM
{
	public ISessionFeature? SessionFeature { get; set; }
	public IRequestCookieCollection? RequestCookieCollection { get; set; }
	public Dictionary<string, string>? RequestCookieDictionary { get; set; }
	public Dictionary<string, string>? SessionKeysDictionary { get; set; }
	public string? SessionCookie { get; set; }
	public string? DecryptedSessionCookie { get; set; }
}