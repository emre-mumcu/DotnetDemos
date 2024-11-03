/// dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddMvc()
	.AddRazorRuntimeCompilation()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.WriteIndented = true;
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	})
;

builder.Services.AddSession(options =>
{
	options.Cookie.Name = Literals.Session_CookieName;
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddDataProtection();

var app = builder.Build();

app.UseStaticFiles();

app.UseSession();


/*
app.Use(async (context, next) =>
{
	if (context.Session.IsAvailable && context.Session.Get(Literals.Session_Key_SessionStart) == null) context.Session.Set<DateTime>(Literals.Session_Key_SessionStart, DateTime.UtcNow);
	await next(context);
});
*/

// app.UseMiddleware<SessionProfileMiddleware>();

app.UseRouting();

app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();

public static partial class Literals
{
	public static string Session_CookieName { get; set; } = "MySessionCookie";
	public static string Session_Key_SessionStart { get; set; } = "SESSION_START";
}

public static class SessionExtensions
{
	public static void Set<T>(this ISession session, string key, T value)
	{
		session.Set(key, JsonSerializer.SerializeToUtf8Bytes(value));
	}

	public static T? Get<T>(this ISession session, string key)
	{
		session.TryGetValue(key, out byte[]? dataByte);

		string? data = dataByte != null ? Encoding.UTF8.GetString(dataByte) : null;

		return data == null ? default(T) : JsonSerializer.Deserialize<T>(data);
	}
}

public class SessionProfileMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<SessionProfileMiddleware> _logger;
	public SessionProfileMiddleware(RequestDelegate next, ILogger<SessionProfileMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Request
		{
			if (context.Session.IsAvailable && context.Session.Get("SESSION_START") != null) 
				_logger.LogInformation($"Session {context.Session.Id} is started at {context.Session.Get<DateTime>("SESSION_START")}");	
		}

		// Call the next middleware/component
		await _next(context);

		// Response		
		{

		}
	}
}