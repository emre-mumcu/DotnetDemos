using Microsoft.EntityFrameworkCore;
using src.App_Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LoggingDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Logging.ClearProviders();



// builder.Services.AddSingleton<ILoggerProvider>(sp => new CustomLoggerProvider(() => sp.GetRequiredService<LoggingDbContext>()));
//builder.Services.AddSingleton<ILoggerProvider>(sp => new CustomLoggerProvider(sp.GetRequiredService<LoggingDbContext>()));
// builder.Logging.ClearProviders();
// builder.Logging.AddProvider(new CustomLoggerProvider(builder.Services.BuildServiceProvider().GetRequiredService<LoggingDbContext>()));
// builder.Services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();
// builder.Services.AddSingleton<IService>(serviceProvider => new Service(serviceProvider.GetRequiredService<IOtherService>(), serviceProvider.GetRequiredService<IAnotherOne>(), ""));
// builder.Services.AddSingleton<IService>(serviceProvider => ActivatorUtilities.CreateInstance<Service>(serviceProvider, ""););

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
