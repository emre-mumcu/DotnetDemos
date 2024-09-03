# DbContext Lifetime, Configuration, and Initialization

## The DbContext lifetime

The lifetime of a DbContext begins when the instance is created and ends when the instance is disposed. A DbContext instance is designed to be used for a single unit-of-work. This means that the lifetime of a DbContext instance is usually very short.

**Important**

* It is very important to dispose the DbContext after use. This ensures both that any unmanaged resources are freed, and that any events or other hooks are unregistered so as to prevent memory leaks in case the instance remains referenced.
* DbContext is not thread-safe. Do not share contexts between threads. Make sure to await all async calls before continuing to use the context instance.
* An InvalidOperationException thrown by EF Core code can put the context into an unrecoverable state. Such exceptions indicate a program error and are not designed to be recovered from.

## DbContext in dependency injection for ASP.NET Core

In many web applications, each HTTP request corresponds to a single unit-of-work. This makes tying the context lifetime to that of the request a good default for web applications.

ASP.NET Core applications are configured using dependency injection. EF Core can be added to this configuration using AddDbContext in the ConfigureServices method of Startup.cs. For example:

```cs
    builder.Services.AddDbContext<ApplicationDbContext>(options => 
		options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
    builder.Services.AddDbContext<ApplicationDbContext>(options => 
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

This example registers a DbContext subclass called ApplicationDbContext as a scoped service in the ASP.NET Core application service provider (a.k.a. the dependency injection container). The context is configured to use the SQL Server database provider and will read the connection string from ASP.NET Core configuration. It typically does not matter where in ConfigureServices the call to AddDbContext is made.

The ApplicationDbContext class must expose a public constructor with a `DbContextOptions<ApplicationDbContext>` parameter. This is how context configuration from AddDbContext is passed to the DbContext. For example:

```cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
```

ApplicationDbContext can then be used in ASP.NET Core controllers or other services through constructor injection. For example:

```cs
public class MyController
{
    private readonly ApplicationDbContext _context;

    public MyController(ApplicationDbContext context)
    {
        _context = context;
    }
}
```

The final result is an ApplicationDbContext instance created for each request and passed to the controller to perform a unit-of-work before being disposed when the request ends.

## Simple DbContext initialization with 'new'

DbContext instances can be constructed in the normal .NET way, for example with new in C#. Configuration can be performed by overriding the OnConfiguring method, or by passing options to the constructor. For example:

```cs
public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");
    }
}
```

This pattern also makes it easy to pass configuration like the connection string via the DbContext constructor. For example:

```cs
public class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}
```

Alternately, DbContextOptionsBuilder can be used to create a DbContextOptions object that is then passed to the DbContext constructor. This allows a DbContext configured for dependency injection to also be constructed explicitly. For example, when using ApplicationDbContext defined for ASP.NET Core web apps above:

The DbContextOptions can be created and the constructor can be called explicitly:

```cs
var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0")
    .Options;

using var context = new ApplicationDbContext(contextOptions);
```

## Using a DbContext factory

Some application types (e.g. ASP.NET Core Blazor) use dependency injection but do not create a service scope that aligns with the desired DbContext lifetime. Even where such an alignment does exist, the application may need to perform multiple units-of-work within this scope. For example, multiple units-of-work within a single HTTP request.

In these cases, AddDbContextFactory can be used to register a factory for creation of DbContext instances. For example:

```cs
services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0"));
```

The ApplicationDbContext class must expose a public constructor with a `DbContextOptions<ApplicationDbContext>` parameter. This is the same pattern as used in the traditional ASP.NET Core section above.

The DbContextFactory factory can then be used in other services through constructor injection. For example:

```cs
private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

public MyController(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    _contextFactory = contextFactory;
}
```

The injected factory can then be used to construct DbContext instances in the service code. For example:

```cs
public void DoSomething()
{
    using (var context = _contextFactory.CreateDbContext())
    {
        // ...
    }
}
```

Notice that the DbContext instances created in this way are not managed by the application's service provider and therefore must be disposed by the application.

### DbContextOptions

The starting point for all DbContext configuration is DbContextOptionsBuilder. There are three ways to get this builder:

* In AddDbContext and related methods
* In OnConfiguring
* Constructed explicitly with new

Examples of each of these are shown in the preceding sections. The same configuration can be applied regardless of where the builder comes from. In addition, OnConfiguring is always called regardless of how the context is constructed. This means OnConfiguring can be used to perform additional configuration even when AddDbContext is being used.

### Configuring the database provider

Each DbContext instance must be configured to use one and only one database provider. (Different instances of a DbContext subtype can be used with different database providers, but a single instance must only use one.) A database provider is configured using a specific Use* call. For example, to use the SQL Server database provider:

```cs
public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");
    }
}
```

These Use* methods are extension methods implemented by the database provider. This means that the database provider NuGet package must be installed before the extension method can be used.

EF Core database providers make extensive use of extension methods. If the compiler indicates that a method cannot be found, then make sure that the provider's NuGet package is installed and that you have using Microsoft.EntityFrameworkCore; in your code.

* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Cosmos
* Microsoft.EntityFrameworkCore.Sqlite
* Microsoft.EntityFrameworkCore.InMemory
* Npgsql.EntityFrameworkCore.PostgreSQL
* Pomelo.EntityFrameworkCore.MySql
* Oracle.EntityFrameworkCore

The EF Core in-memory database is not designed for production use. In addition, it may not be the best choice even for testing. See Testing Code That Uses EF Core for more information.

Optional configuration specific to the database provider is performed in an additional provider-specific builder.

### Other DbContext configuration

Other DbContext configuration can be chained either before or after (it makes no difference which) the Use* call. For example, to turn on sensitive-data logging:

```cs
public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");
    }
}
```

The following table contains examples of common methods called on DbContextOptionsBuilder.

* UseQueryTrackingBehavior
* LogTo
* UseLoggerFactory
* EnableSensitiveDataLogging
* EnableDetailedErrors
* ConfigureWarnings
* AddInterceptors
* UseLazyLoadingProxies
* UseChangeTrackingProxies

UseLazyLoadingProxies and UseChangeTrackingProxies are extension methods from the Microsoft.EntityFrameworkCore.Proxies NuGet package. This kind of ".UseSomething()" call is the recommended way to configure and/or use EF Core extensions contained in other packages.

### DbContextOptions versus DbContextOptions<TContext>
Most DbContext subclasses that accept a DbContextOptions should use the generic `DbContextOptions<TContext>` variation. For example:

```cs
public sealed class SealedApplicationDbContext : DbContext
{
    public SealedApplicationDbContext(DbContextOptions<SealedApplicationDbContext> contextOptions) : base(contextOptions) { }
}
```

This ensures that the correct options for the specific DbContext subtype are resolved from dependency injection, even when multiple DbContext subtypes are registered.

**Tip**

Your DbContext does not need to be sealed, but sealing is best practice to do so for classes not designed to be inherited from. However, if the DbContext subtype is itself intended to be inherited from, then it should expose a protected constructor taking a non-generic DbContextOptions. For example:

```cs
public abstract class ApplicationDbContextBase : DbContext
{
    protected ApplicationDbContextBase(DbContextOptions contextOptions) : base(contextOptions) { }
}
```

A DbContext subclass intended to be both instantiated and inherited from should expose both forms of constructor. For example:

```cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : base(contextOptions) { }

    protected ApplicationDbContext(DbContextOptions contextOptions) : base(contextOptions) { }
}
```

### Avoiding DbContext threading issues
Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads. Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute in parallel.

When EF Core detects an attempt to use a DbContext instance concurrently, you'll see an InvalidOperationException with a message like this:

*A second operation started on this context before a previous operation completed. This is usually caused by different threads using the same instance of DbContext, however instance members are not guaranteed to be thread safe.*

When concurrent access goes undetected, it can result in undefined behavior, application crashes and data corruption.

# Design-time DbContext Creation

Some of the EF Core Tools commands (for example, the Migrations commands) require a derived DbContext instance to be created at design time in order to gather details about the application's entity types and how they map to a database schema. In most cases, it is desirable that the DbContext thereby created is configured in a similar way to how it would be configured at run time.

There are various ways the tools try to create the DbContext:

## From application services

If your startup project uses the ASP.NET Core Web Host or .NET Core Generic Host, the tools try to obtain the DbContext object from the application's service provider.

The tools first try to obtain the service provider by invoking Program.CreateHostBuilder(), calling Build(), then accessing the Services property.

## Using a constructor with no parameters

If the DbContext can't be obtained from the application service provider, the tools look for the derived DbContext type inside the project. Then they try to create an instance using a constructor with no parameters. This can be the default constructor if the DbContext is configured using the OnConfiguring method.

## From a design-time factory

You can also tell the tools how to create your DbContext by implementing the Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<TContext> interface: If a class implementing this interface is found in either the same project as the derived DbContext or in the application's startup project, the tools bypass the other ways of creating the DbContext and use the design-time factory instead.

```cs
public class BloggingContextFactory : IDesignTimeDbContextFactory<BloggingContext>
{
    public BloggingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
        optionsBuilder.UseSqlite("Data Source=blog.db");

        return new BloggingContext(optionsBuilder.Options);
    }
}
```

A design-time factory can be especially useful if you need to configure the DbContext differently for design time than at run time, if the DbContext constructor takes additional parameters are not registered in DI, if you are not using DI at all, or if for some reason you prefer not to have a CreateHostBuilder method in your ASP.NET Core application's Main class.

## Args

Both IDesignTimeDbContextFactory<TContext>.CreateDbContext and Program.CreateHostBuilder accept command line arguments.

You can specify these arguments from the tools:

```zsh
% dotnet ef database update -- --environment Production
```

# References

* https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
* https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli