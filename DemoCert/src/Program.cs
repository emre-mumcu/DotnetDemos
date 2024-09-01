var builder = WebApplication.CreateBuilder(args);

//X509Certificate2 cert1 = new X509Certificate2("certificate.pfx");
//X509Certificate2 cert2 = new X509Certificate2("certificate.pfx", "12345678");

builder.Configuration.AddJsonFile($"kestrel.p12.json", optional: false, reloadOnChange: false);
// builder.Configuration.AddJsonFile($"kestrel.pem.json", optional: false, reloadOnChange: false);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();

/*
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
*/