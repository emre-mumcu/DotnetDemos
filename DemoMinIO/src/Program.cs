using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMinio(configureClient => {
	var mc = builder.Configuration.GetSection("Minio").Get<MinioConfig>();
	configureClient
		.WithEndpoint(mc!.Endpoint)
		.WithCredentials(mc!.AccessKey, mc!.SecretKey)
		.WithSSL()
		.Build();
	}
);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

public class MinioConfig
{
	public string Endpoint { get; set; } = null!;
	public string AccessKey { get; set; } = null!;
	public string SecretKey { get; set; } = null!;
	public string BucketName { get; set; } = null!;
}