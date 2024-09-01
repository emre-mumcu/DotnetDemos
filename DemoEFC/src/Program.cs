using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using src.App_Data;
using src.App_Data.Context;
using src.App_Lib;

try
{
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = AppConstants.Session_Cookie_Name;
            options.IdleTimeout = TimeSpan.FromMinutes(AppConstants.Session_IdleTimeout);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        IMvcBuilder mvcBuilder = builder.Services.AddMvc();

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        });

        // Use session based TempData instead of cookie based TempData
        mvcBuilder.AddSessionStateTempDataProvider();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        mvcBuilder.AddJsonOptions(options => { 
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true; 
            });

        // dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
        mvcBuilder.AddNewtonsoftJson(options =>
         {
             // options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
             options.SerializerSettings.ContractResolver = new DefaultContractResolver();
             options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
         });

        builder.Services.AddDataProtection();

        {   // DbContext:

            // In this type of service registration, connection string is NOT provided to DI.
            // It must be provided in DbContext's OnConfiguring method.
            builder.Services.AddDbContext<AppDbContext>();

            // In this type of service registration, connection string is provided to DI.
            // If DbContext is created by DI, connection string is present in the instance.
            // But if user manually creates DbContext, the connection string must also be provided in DbContext's OnConfiguring method
            // builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString: ""));

            // Manually            
            // builder.Services.AddScoped(x => { return new AppDbContext(); });
        }

        builder.Services.Configure<RazorViewEngineOptions>(options => {
            options.ViewLocationExpanders.Add(new ViewLocationExpander());
        });
    }

    var app = builder.Build();

    App.Instance._WebHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();

    {
        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        app.UseAuthorization();

        // app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllers(); // MapControllers is called to map attribute routed controllers

        app.MapDefaultControllerRoute();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
    }

    await DataSeeder.SeedData(app.Services);

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
                await context.Response.WriteAsync($"Application Error {ex?.Message} {ex?.InnerException?.Message}");
            });
        });
    }).Build().Run();
}

public sealed class App
{
    // https://csharpindepth.com/articles/singleton (Sixth version)

    private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());

    public static App Instance { get { return appInstance.Value; } }

    private App() 
    {
        _DataConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("data.json", true)
            .Build();
    }

    public IConfiguration _DataConfiguration { get; set; }
    public IWebHostEnvironment _WebHostEnvironment { get; set; } = null!;
}