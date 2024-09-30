using Minio;
using src.App_Lib;
using src.Models;

var builder = WebApplication.CreateBuilder(args);

// Register IMinioClient to IoC 
builder.Services.AddMinio(configureClient => {
	var mc = builder.Configuration.GetSection("Minio").Get<MinioConfigModel>();
	configureClient
		.WithEndpoint(mc!.Endpoint)
		.WithCredentials(mc!.AccessKey, mc!.SecretKey)
		.WithSSL()
		.Build();
	}
);

// Register MinioManager to IoC
builder.Services.AddSingleton<MinioManager>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

