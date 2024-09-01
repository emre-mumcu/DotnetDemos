using src.Lib;

var builder = WebApplication.CreateBuilder(args);

{   // Configuration
    
    builder.Configuration
        .AddJsonFile($"data.json", optional: false, reloadOnChange: false)
        .AddJsonFile($"data.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    ;
    
    builder.Configuration.AddInMemoryCollection(App_Constants.App_Keys);
    
    // builder.Configuration.AddEnvironmentVariables(); // already added by WebApplication.CreateBuilder as default    

    // builder.Configuration.AddCommandLine(args); // already added by WebApplication.CreateBuilder as default
}

{   // Options
    builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
}

// dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
builder.Services.AddMvc().AddRazorRuntimeCompilation();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// To use routes mapped to controllers as attribute
// MapControllers doesn't make any assumptions about routing and will rely on the user doing attribute routing
app.MapControllers();

// Uses conventional routing (most often used in an MVC application), and sets up the URL route pattern.
// It shorthands the configuration of the default pattern: 
// app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();

App.Instance.WebHostEnvironment = app.Environment;

app.Run();