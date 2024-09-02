# Logging in .NET Core and ASP.NET Core

## Logging providers

The default ASP.NET Core web app templates call WebApplication.CreateBuilder, which adds the following logging providers:

* Console
* Debug
* EventSource
* EventLog: Windows only

```cs
// Configure logging

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddEventLog();

// Alternatively, the preceding logging code can be written as follows:

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});
```

## Built-in logging providers
ASP.NET Core includes the following logging providers as part of the shared framework:

* Console
* Debug
* EventSource
* EventLog

The following logging providers are shipped by Microsoft, but not as part of the shared framework. They must be installed as additional nuget.

* AzureAppServicesFile and AzureAppServicesBlob
* ApplicationInsights

ASP.NET Core doesn't include a logging provider for writing logs to files. To write logs to files from an ASP.NET Core app, consider using a third-party logging provider.

### Console
The Console provider logs output to the console.

### Debug
The Debug provider writes log output by using the System.Diagnostics.Debug class. Calls to System.Diagnostics.Debug.WriteLine write to the Debug provider.

### Event Source
The EventSource provider writes to a cross-platform event source with the name Microsoft-Extensions-Logging. On Windows, the provider uses ETW.

### Windows EventLog
The EventLog provider sends log output to the Windows Event Log. Unlike the other providers, the EventLog provider does not inherit the default non-provider settings. If EventLog log settings aren't specified, they default to LogLevel.Warning.

AddEventLog overloads can pass in EventLogSettings. If null or not specified, the following default settings are used:

* LogName: "Application"
* SourceName: ".NET Runtime"
* MachineName: The local machine name is used.

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddEventLog(eventLogSettings =>
{
    eventLogSettings.SourceName = "MyLogs";
});
```

## Third-party logging providers

* elmah.io
* Gelf
* JSNLog
* KissLog.net
* Log4Net
* NLog
* PLogger
* Sentry
* Serilog
* Stackdriver

## No asynchronous logger methods

Logging should be so fast that it isn't worth the performance cost of asynchronous code. If a logging data store is slow, don't write to it directly. Consider writing the log messages to a fast store initially, then moving them to the slow store later. For example, when logging to SQL Server, don't do so directly in a Log method, since the Log methods are synchronous. Instead, synchronously add log messages to an in-memory queue and have a background worker pull the messages out of the queue to do the asynchronous work of pushing data to SQL Server.

## ILogger and ILoggerFactory
The `ILogger<TCategoryName>` and `ILoggerFactory` interfaces and implementations are included in the .NET Core SDK. They are also available in the following NuGet packages:

* The interfaces are in Microsoft.Extensions.Logging.Abstractions.
* The default implementations are in Microsoft.Extensions.Logging.

## Apply log filter rules in code

Although the preferred approach for setting log filter rules is by using Configuration, the following example shows how to register filter rules in code:

```cs
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("System", LogLevel.Debug);
builder.Logging.AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Information);
builder.Logging.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Trace);
```

`logging.AddFilter("System", LogLevel.Debug)` specifies the System category and log level Debug. The filter is applied to all providers because a specific provider was not configured.

`AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Information)` specifies:

* The Debug logging provider.
* Log level Information and higher.
* All categories starting with "Microsoft".

# Implement a custom logging provider in .NET

There are many logging providers available for common logging needs. You may need to implement a custom ILoggerProvider when one of the available providers doesn't suit your application needs.

## Custom Logger for Logging into a Database

dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

dotnet ef migrations add Mig0
dotnet ef database update

appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LoggingDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```


# References

https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0
https://github.com/dotnet/AspNetCore.Docs/issues/11801
https://learn.microsoft.com/en-us/dotnet/core/extensions/custom-logging-provider
https://www.crowdstrike.com/guides/net-logging/custom-logging-providers/





```cs
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    
}

```

ILogger supports various log levels which can be used to differentiate the severity of logs. Common log levels are:

Trace: Detailed information, typically for debugging.
Debug: Information used for debugging.
Information: General application flow information.
Warning: Indication of potential issues.
Error: Errors that occur during application execution.
Critical: Critical errors causing the application to crash or terminate.

You can also configure logging settings in appsettings.json to control log levels and outputs.

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}