using System.Security.Cryptography.X509Certificates;
using src.App_Lib;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//X509Certificate2 cert1 = new X509Certificate2("certificate.pfx");
X509Certificate2 cert2 = new X509Certificate2("certificate.pfx", "12345678");

bool useAppsettings = false;

if (!useAppsettings)
{
	// Configure Kestrel 
	builder.WebHost.ConfigureKestrel(options =>
	{
		// Listen on port 5000 for HTTP
		options.ListenAnyIP(5000);

		// Listen on port 5001 for HTTPS
		options.ListenAnyIP(5001, listenOptions =>
		{

			// Use default dotnet development certificate
			// listenOptions.UseHttps();

			// Use custom certificate
			listenOptions.UseHttps("certificate.pfx", "12345678");
		});

		options.Limits.MaxConcurrentConnections = 100;

		options.Limits.MaxRequestBodySize = 10 * 1024; // 10 KB
	});
}
else
{
	// Configure Kestrel using appsettings.json
	builder.WebHost.ConfigureKestrel((context, options) =>
	{
		var kestrelConfig = context.Configuration.GetSection("Kestrel");
		options.Configure(kestrelConfig);
	});
}

builder.Configuration.AddJsonFile($"data.json", optional: false, reloadOnChange: false);

builder.Services.AddMvc().AddRazorRuntimeCompilation();

// Configure IConnectionMultiplexer (Redis) for DI
// By this way you can use IoC to get ConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
	var configurationOptions = ConfigurationOptions.Parse(builder.Configuration["Redis:Configuration"]!);
	return ConnectionMultiplexer.Connect(configurationOptions);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// To use routes mapped to controllers as attribute
// MapControllers doesn't make any assumptions about routing and will rely on the user doing attribute routing
app.MapControllers();

// Uses conventional routing (most often used in an MVC application), and sets up the URL route pattern.
// It shorthands the configuration of the default pattern: 
// app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();

App.Instance.Environment = app.Environment;

App.Instance.Configuration = app.Configuration;

app.Run();