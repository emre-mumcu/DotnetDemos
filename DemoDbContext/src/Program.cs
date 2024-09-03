using Microsoft.EntityFrameworkCore;
using src.App_Lib;

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration.AddJsonFile($"data.json", optional: true, reloadOnChange: false);

builder.Services.AddControllersWithViews();

// builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppDbContext>((provider, options) => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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
