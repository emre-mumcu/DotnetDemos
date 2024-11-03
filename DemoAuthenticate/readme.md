# Use cookie authentication without ASP.NET Core Identity

ASP.NET Core Identity is a complete, full-featured authentication provider for creating and maintaining logins. However, a cookie-based authentication provider without ASP.NET Core Identity can be used.

## Add cookie authentication

```cs
builder.Services
	.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

app.UseAuthentication();
app.UseAuthorization();
```

AuthenticationScheme passed to AddAuthentication sets the default authentication scheme for the app. 

AuthenticationScheme is useful when there are multiple instances of cookie authentication and the app needs to authorize with a specific scheme. 

Setting the AuthenticationScheme to CookieAuthenticationDefaults.AuthenticationScheme provides a value of "Cookies" for the scheme. Any string value can be used that distinguishes the scheme.

The app's authentication scheme is different from the app's cookie authentication scheme. When a cookie authentication scheme isn't provided to AddCookie, it uses CookieAuthenticationDefaults.AuthenticationScheme. 

The authentication cookie's IsEssential property is set to true by default. Authentication cookies are allowed when a site visitor hasn't consented to data collection.

The CookieAuthenticationOptions class is used to configure the authentication provider options. Configure CookieAuthenticationOptions in the AddCookie method.

## Cookie Policy Middleware

The Cookie Policy Middleware's UseCookiePolicy method enables cookie policy capabilities. Middleware is processed in the order it's added.

```cs
app.UseCookiePolicy(cookiePolicyOptions);
```

Use CookiePolicyOptions provided to the Cookie Policy Middleware to control global characteristics of cookie processing and hook into cookie processing handlers when cookies are appended or deleted.

The default MinimumSameSitePolicy value is SameSiteMode.Lax to permit OAuth2 authentication. To strictly enforce a same-site policy of SameSiteMode.Strict, set the MinimumSameSitePolicy. Although this setting breaks OAuth2 and other cross-origin authentication schemes, it elevates the level of cookie security for other types of apps that don't rely on cross-origin request processing.

```cs
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
```

## Create an authentication cookie

To create a cookie holding user information, construct a ClaimsPrincipal. The user information is serialized and stored in the cookie.

Create a ClaimsIdentity with any required Claims and call SignInAsync to sign in the user.

```cs
var claims = new List<Claim>
{
	new Claim(ClaimTypes.Name, user.Email),
	new Claim(ClaimTypes.Role, "Administrator"),
};

var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

var authProperties = new AuthenticationProperties
{
	//AllowRefresh = <bool>,
	// Refreshing the authentication session should be allowed.

	//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
	// The time at which the authentication ticket expires. A 
	// value set here overrides the ExpireTimeSpan option of 
	// CookieAuthenticationOptions set with AddCookie.

	//IsPersistent = true,
	// Whether the authentication session is persisted across 
	// multiple requests. When used with cookies, controls
	// whether the cookie's lifetime is absolute (matching the
	// lifetime of the authentication ticket) or session-based.

	//IssuedUtc = <DateTimeOffset>,
	// The time at which the authentication ticket was issued.

	//RedirectUri = <string>
	// The full path or absolute URI to be used as an http 
	// redirect response value.
};

await HttpContext.SignInAsync(
	CookieAuthenticationDefaults.AuthenticationScheme, 
	new ClaimsPrincipal(claimsIdentity), 
	authProperties
);
```

SignInAsync creates an encrypted cookie and adds it to the current response. If AuthenticationScheme isn't specified, the default scheme is used.

ASP.NET Core's Data Protection system is used for encryption. For an app hosted on multiple machines, load balancing across apps, or using a web farm, configure data protection to use the same key ring and app identifier.

## Sign out

To sign out the current user and delete their cookie, call SignOutAsync:

```cs
// Clear the existing external cookie
await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
```

If CookieAuthenticationDefaults.AuthenticationScheme or "Cookies" isn't used as the scheme, supply the scheme used when configuring the authentication provider. Otherwise, the default scheme is used. For example, if "ContosoCookie" is used as the scheme, supply the scheme used when configuring the authentication provider.

When the browser closes it automatically deletes session based cookies (non-persistent cookies), but no cookies are cleared when an individual tab is closed. The server is not notified of tab or browser close events.

## React to back-end changes

Once a cookie is created, the cookie is the single source of identity. If a user account is disabled in back-end systems:

* The app's cookie authentication system continues to process requests based on the authentication cookie.
* The user remains signed into the app as long as the authentication cookie is valid.

The `ValidatePrincipal` event can be used to intercept and override validation of the cookie identity. Validating the cookie on every request mitigates the risk of revoked users accessing the app.

To implement an override for the ValidatePrincipal event, write a method with the following signature in a class that derives from CookieAuthenticationEvents:

```cs
builder.Services
	.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
		options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();	
```

Consider a situation in which the user's name is updated—a decision that doesn't affect security in any way. If you want to non-destructively update the user principal, call context.ReplacePrincipal and set the context.ShouldRenew property to true.

The approach described here is triggered on every request. Validating authentication cookies for all users on every request can result in a large performance penalty for the app.

## Persistent cookies

You may want the cookie to persist across browser sessions. This persistence should only be enabled with explicit user consent with a "Remember Me" checkbox on sign in or a similar mechanism.

The following code snippet creates an identity and corresponding cookie that survives through browser closures. Any sliding expiration settings previously configured are honored. If the cookie expires while the browser is closed, the browser clears the cookie once it's restarted.

Set IsPersistent to true in AuthenticationProperties:

```cs
await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(claimsIdentity),
    new AuthenticationProperties { IsPersistent = true });
```

## Absolute cookie expiration

An absolute expiration time can be set with ExpiresUtc. To create a persistent cookie, IsPersistent must also be set. Otherwise, the cookie is created with a session-based lifetime and could expire either before or after the authentication ticket that it holds. When ExpiresUtc is set, it overrides the value of the ExpireTimeSpan option of CookieAuthenticationOptions, if set.

The following code snippet creates an identity and corresponding cookie that lasts for 20 minutes. This ignores any sliding expiration settings previously configured.

```cs
await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(claimsIdentity),
    new AuthenticationProperties { 
		IsPersistent = true,
		ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
	});
```

# References

* https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
* https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio
* https://github.com/dotnet/aspnetcore/blob/v6.0.1/src/Security/Authentication/Cookies/src/CookieAuthenticationDefaults.cs#L11
* https://learn.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-8.0#essential-cookies
* https://github.com/dotnet/aspnetcore/blob/main/src/Security/CookiePolicy/src/CookiePolicyMiddleware.cs
* https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.cookies.cookieauthenticationevents?view=aspnetcore-8.0