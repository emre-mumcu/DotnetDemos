var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc().AddRazorRuntimeCompilation();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

// Enable attribute routing for your controllers, allowing you to define routes directly on the controller
app.MapControllers();

// var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<src.App_Lib.Tools.AppBenchmark>();

app.Run();