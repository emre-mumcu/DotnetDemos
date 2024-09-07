using src.App_Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

builder.Services.AddSingleton(new RabbitMqSenderService(
	rabbitMqConfig["HostName"]!,
	rabbitMqConfig["UserName"]!,
	rabbitMqConfig["Password"]!
));

builder.Services.AddSingleton(new RabbitMqConsumerService(
	rabbitMqConfig["HostName"]!,
	rabbitMqConfig["UserName"]!,
	rabbitMqConfig["Password"]!,
	"test_queue" // Replace with your queue name
));

builder.Services.AddHostedService<RabbitMqBackgroundService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
