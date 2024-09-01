using src.App_Lib;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"data.json", optional: false, reloadOnChange: false);

builder.Services.AddMvc().AddRazorRuntimeCompilation();

// Register IConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
	var configurationOptions = ConfigurationOptions.Parse(builder.Configuration["Redis:Configuration"]!);
	return ConnectionMultiplexer.Connect(configurationOptions);
});

{   // Distributed session using Redis

	builder.Services.AddStackExchangeRedisCache(redisCacheConfig =>
	{		
		// var configurationOptions = ConfigurationOptions.Parse(builder.Configuration["Redis:Configuration"]!);
		// redisCacheConfig.ConfigurationOptions = configurationOptions;

		redisCacheConfig.InstanceName = "DemoRedisInstance";
		redisCacheConfig.Configuration = builder.Configuration["Redis:Configuration"]!;
		redisCacheConfig.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
		{
			AbortOnConnectFail = true,
			EndPoints = { redisCacheConfig.Configuration }
		};
	});

	builder.Services.ConfigureApplicationCookie(options =>
	{
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
		options.LoginPath = "/Auth/Login";
		options.AccessDeniedPath = "/Dashboard";
		options.LogoutPath = "/Auth/Logout";
		options.SlidingExpiration = true;
		options.Cookie.IsEssential = true;
	});

	builder.Services.AddSession(options =>
	{
		options.Cookie.HttpOnly = true;
		options.Cookie.Name = "demo_redis_session";
		// since ConfigureApplicationCookie is set
		//options.Cookie.Expiration = TimeSpan.FromMinutes(30);
		options.Cookie.IsEssential = true;
		options.IdleTimeout = TimeSpan.FromMinutes(30);
	});
}

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

App.Instance.Environment = app.Environment;

App.Instance.Configuration = app.Configuration;

app.Run();