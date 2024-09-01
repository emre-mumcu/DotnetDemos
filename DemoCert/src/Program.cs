var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"kestrel.p12.json", optional: false, reloadOnChange: false);
// builder.Configuration.AddJsonFile($"kestrel.pem.json", optional: false, reloadOnChange: false);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();