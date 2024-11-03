// dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation

using DemoAuthenticate.AppLib;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddMvc()
	.AddRazorRuntimeCompilation()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.WriteIndented = true;
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
		options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
	})
;

builder.Services.AddSession(options =>
{
	options.Cookie.Name = Literals.SessionCookie_Name;
	options.IdleTimeout = TimeSpan.FromMinutes(Literals.SessionCookie_IdleTimeout);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor(); // builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
	options.Cookie.Path = Literals.AuthenticationCookie_Path;
	options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	options.Cookie.HttpOnly = true;
	options.Cookie.Name = Literals.AuthenticationCookie_Name;
	options.LoginPath = Literals.AuthenticationCookie_LoginPath;
	options.LogoutPath = Literals.AuthenticationCookie_LogoutPath;
	options.AccessDeniedPath = Literals.AuthenticationCookie_AccessDeniedPath;
	options.ReturnUrlParameter = Literals.AuthenticationCookie_ReturnUrl;
	options.EventsType = typeof(CustomCookieAuthenticationEvents);
	options.Events.OnRedirectToLogin = (context) =>
	{
		return Task.CompletedTask;
	};
});

builder.Services.AddAuthorization(options =>
{
	options.DefaultPolicy = AuthorizationPolicyLibrary.defaultPolicy;
	options.FallbackPolicy = AuthorizationPolicyLibrary.fallbackPolicy;
	options.AddPolicy(nameof(AuthorizationPolicyLibrary.userPolicy), AuthorizationPolicyLibrary.userPolicy);
	options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Administrator"));
});

builder.Services.AddSingleton<IAuthorizationHandler, UserHandler>();

builder.Services.AddSingleton<IAuthenticate, TestAuthenticate>();

builder.Services.AddSingleton<IAuthorize, TestAuthorize>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.Use(async (context, next) =>
{
	if (context.Session.IsAvailable) context.Session.SetLastRequestTimeStamp();
	await next(context);
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();