using Microsoft.EntityFrameworkCore;
using src.App_Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"data.json", optional: false, reloadOnChange: false);

builder.Services.AddDbContext<LoggingDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// builder.Logging.ClearProviders();

// ServiceProvider serviceProvider = new ServiceCollection()
// 	.AddDbContext(options => options.UseSqlite(connectionString: builder.Configuration.GetConnectionString("DefaultConnection")))
// 	.BuildServiceProvider();

// builder.Logging.AddProvider(new CustomLoggerProvider(serviceProvider.GetRequiredService<LoggingDbContext>()));

builder.Services.AddControllersWithViews();

var app = builder.Build();

App.Instance.WebHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();


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