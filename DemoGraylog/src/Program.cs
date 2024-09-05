using System.Reflection;
using Gelf.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Logging.AddGelf(options =>
{
    var configuration = builder.Configuration;
    var gc = builder.Configuration.GetSection("GELF").Get<GelfConfiguration>();
    options.Host = gc!.Host;
    options.Port = gc!.Port;
    options.Protocol = gc!.Protocol;
    options.LogSource = gc.LogSource;
    options.AdditionalFields["instance-guid"] = Guid.NewGuid().ToString();
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

public class GelfConfiguration
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 12201;
    public GelfProtocol Protocol { get; set; } = GelfProtocol.Udp;
    public string? LogSource { get; set; } = Assembly.GetExecutingAssembly().GetName().Name;
}