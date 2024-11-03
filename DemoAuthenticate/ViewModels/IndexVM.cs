using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace DemoAuthenticate.ViewModels;

public class IndexVM
{
	public ISessionFeature? SessionFeature { get; set; }
	public SessionOptions? SessionOptions { get; set; }
	public IRequestCookieCollection? RequestCookieCollection { get; set; }	
	public Dictionary<string, string>? RequestCookieDictionary { get; set; }
	public Dictionary<string, string>? SessionKeysDictionary { get; set; }
	public Dictionary<string, string>? AllCookies { get; set; }
	public string? SessionCookie { get; set; }
	public string? DecryptedSessionCookie { get; set; }
	public string? AuthCookie { get; set; }
	public string? DecryptedAuthCookie { get; set; }
	public AuthenticationTicket? AuthenticationTicketFromCookie { get; set; }
	public AuthenticationTicket? AuthenticationTicketFromAuthResult { get; set; }
}
