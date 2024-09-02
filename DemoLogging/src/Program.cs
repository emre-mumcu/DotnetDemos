using Microsoft.EntityFrameworkCore;
using src.App_Lib;

try
{
	var commandLine = System.Text.Json.JsonSerializer.Serialize(Environment.CommandLine);

	var IsMigration = commandLine.Contains("ef.dll");

	var builder = WebApplication.CreateBuilder(args);

	builder.Configuration.AddJsonFile($"data.json", optional: false, reloadOnChange: false);

	builder.Services.AddDbContext<LoggingDbContext>(options =>
		options.UseSqlite(builder.Configuration.GetConnectionString("LoggingConnection"))
	);

	builder.Logging.ClearProviders();

	builder.Logging.AddConsole();

	if (!IsMigration)
	{
		// builder.Services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();
		builder.Logging.AddProvider(new CustomLoggerProvider());
	}

	builder.Services.AddControllersWithViews();

	var app = builder.Build();

	var logger = app.Services.GetRequiredService<ILogger<Program>>();

	App.Instance.WebHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();

	app.UseHttpsRedirection();

	app.UseStaticFiles();

	app.UseRouting();

	app.UseAuthorization();

	app.MapDefaultControllerRoute();

	app.Run();
}
catch (Exception ex)
{
	Host.CreateDefaultBuilder(args)
		.ConfigureServices(services => { services.AddMvc(); })
		.ConfigureWebHostDefaults(webBuilder =>
		{
			webBuilder.Configure((ctx, app) =>
			{
				app.Run(async (context) =>
				{
					await context.Response.WriteAsync($"Error in application: {ex.Message} {ex.InnerException?.Message}");
				});
			});
		})
		.Build()
		.Run();
}

public sealed class App
{
	private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());

	public static App Instance { get { return appInstance.Value; } }

	private App()
	{
		DataConfiguration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("data.json", true)
			.Build();
	}

	public IConfiguration DataConfiguration { get; set; }

	public IWebHostEnvironment? WebHostEnvironment { get; set; }
}