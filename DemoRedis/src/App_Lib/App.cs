namespace src.App_Lib;

public sealed class App
{
	// https://csharpindepth.com/articles/singleton (Sixth version)

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

	public IConfiguration? Configuration { get; set; }

	public IWebHostEnvironment? Environment { get; set; }
}

public class RedisOptions
{
	public string Configuration { get; set; } = null!;
}