// dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation

using DemoAuthenticate.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
	options.Cookie.Name = Literals.SessionCookieName;
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

/* builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = new PathString("/User/Login");
	options.LogoutPath = new PathString("/User/Logout");
	options.AccessDeniedPath = new PathString("/Home/AccessDenied");

	options.Cookie = new()
	{
		Name = "IdentityCookie",
		HttpOnly = true,
		SameSite = SameSiteMode.Lax,
		SecurePolicy = CookieSecurePolicy.Always
	};
	options.SlidingExpiration = true;
	options.ExpireTimeSpan = TimeSpan.FromDays(30);
}); */

builder.Services.AddDataProtection();

// builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
	options.Cookie.Path = "/";
	options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	options.Cookie.HttpOnly = true;
	options.Cookie.Name = Literals.AuthenticationCookieName;
	options.LoginPath = new PathString("/Login");
	options.LogoutPath = "/Logout";
	options.AccessDeniedPath = "/AccessDenied";
	options.ReturnUrlParameter = "ReturnUrl";
	options.EventsType = typeof(CustomCookieAuthenticationEvents);
	options.Events.OnRedirectToLogin = (context) =>
	{
		return Task.CompletedTask;
	};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

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

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
	public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
	{
		// Runs in every request
		Boolean login = context.HttpContext.Session.Get<bool>(Literals.SessionKeyLogin);

		if (context.Principal != null && context.Principal.Identity != null)
		{
			if (!(context.Principal.Identity.IsAuthenticated && login))
			{
				// Bu şekilde kapanmış tarayıcıdan gelenler
				// auth cookieleri olsa bile sessionları değiştiği için
				// yeniden login olmadan işlem yapamazlar!!!
				context.RejectPrincipal();
				await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}
		}
		else
		{
			// TODO: What to do?
		}
	}
}

public static class Literals
{
	public static string SessionCookieName = "OturumKurabiyesi";
	public static string AuthenticationCookieName = "KimlikDogrulamaKurabiyesi";
	public static string SessionKeyLogin = "LOGIN";
}

public static partial class SessionExtensions
{
	public static void Set<T>(this ISession session, string key, T value)
	{
		session.Set(key, System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value));
	}

	public static T? Get<T>(this ISession session, string key)
	{
		session.TryGetValue(key, out byte[]? dataByte);

		string? data = dataByte != null ? System.Text.Encoding.UTF8.GetString(dataByte) : null;

		return data == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(data);
	}
}

public static partial class SessionExtensions
{
	public static void SetLastRequestTimeStamp(this ISession session)
	{
		session.Set<DateTime>("LRTS", DateTime.UtcNow);
	}

	public static DateTime GetLastRequestTimeStamp(this ISession session)
	{
		return session.Get<DateTime>("LRTS");
	}

	public static void SetUserSession(this ISession session, HttpContext context)
	{
		UserVM user = new UserVM()
		{
			SessionId = context.Session.Id,
			SessionStart = DateTime.UtcNow,
		};

		session.Set<UserVM>("USER", user);
		session.Set<bool>(Literals.SessionKeyLogin, true);
	}

	public static UserVM? GetUserSession(this ISession session)
	{
		return session.Get<UserVM>("USER");
	}

	public static void ResetUserSession(this ISession session, UserVM user)
	{
		session.Set<bool>(Literals.SessionKeyLogin, false);
		session.Set<UserVM>("USER", new UserVM());
		session.Remove("USER");
	}
}