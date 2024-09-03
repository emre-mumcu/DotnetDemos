# Chapter-2: Code-First

EF Core's "Code First" approach allows you to define your database schema using C# classes, which EF Core then uses to generate the database schema.

## Entities

An entity in Entity Framework is a class that maps to a database table. This class must be included as a `DbSet<TEntity>` type property in the DbContext class. EF API maps each entity to a table and each property of an entity to a column in the database.

The primitive type properties are called scalar properties. Each scalar property maps to a column in the database table which stores an actual data.

If an entity includes a property of another entity type, it is called a Reference Navigation Property. It points to a single entity and represents multiplicity of one (1) in the entity relationships.

EF API will create a ForeignKey column in the table for the navigation properties that points to a PrimaryKey of another table in the database.

If an entity includes a property of generic collection of an entity type, it is called a collection navigation property. It represents multiplicity of many (\*).

EF API does not create any column for the collection navigation property in the related table of an entity, but it creates a column in the table of an entity of generic collection.

EF API maintains the state of each entity during its lifetime. Each entity has a state based on the operation performed on it via the context class. The entity states are as follows:

-   Added
-   Modified
-   Deleted
-   Unchanged
-   Detached

### One-to-One

```cs
// Principal (parent)
public class Author
{
	public int Id { get; set; }
	public Biography? Biography { get; set; } // Reference navigation to dependent
}

// Dependent (child)
public class Biography
{
	public int Id { get; set; }
	public int AuthorId { get; set; } // Required foreign key property
	public Author Author { get; set; } = null!; // Required reference navigation to principal
}

// For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly as follows:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Author>()
        .HasOne(e => e.Biography)
        .WithOne(e => e.Author)
        .HasForeignKey<AuthorBiography>(e => e.AuthorId)
        .IsRequired();
}
```

### One-to-Many

```cs
// Principal (parent)
public class Blog
{
	public int Id { get; set; }
	public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents
}

// Dependent (child)
public class Post
{
	public int Id { get; set; }
	public int BlogId { get; set; } // Required foreign key property
	public Blog Blog { get; set; } = null!; // Required reference navigation to principal
}

// For cases where the navigations, foreign key, or required/optional nature of the relationship are not discovered by convention, these things can be configured explicitly as follows:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasOne(e => e.Blog)
        .WithMany(e => e.Posts)
        .HasForeignKey(e => e.BlogId)
        .IsRequired();
}
```

### Many-to-Many

```cs
// Basic many-to-many
public class Post
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = [];
}

public class Tag
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = [];
}

// Even though it is not needed, an equivalent explicit configuration for this relationship is shown below:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Posts)
        .UsingEntity("PostsToTagsJoinTable") // optional
		;
}
```

## DbContext

The EF Core DbContext class represents a session with a database and provides an API for communicating with the database with the following capabilities:

* Database Connections
* Data operations such as querying and persistence
* Change Tracking
* Model building
* Data Mapping
* Object caching
* Transaction management

DbContext is not thread-safe. Do not share contexts between threads. Make sure to await all async calls before continuing to use the context instance.

An InvalidOperationException thrown by EF Core code can put the context into an unrecoverable state. Such exceptions indicate a program error and are not designed to be recovered from.


```cs
public partial class AppDbContext : DbContext
{
	public AppDbContext() { }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

		base.OnConfiguring(optionsBuilder);

		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseNpgsql("connection-string");
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

		base.OnModelCreating(modelBuilder);

		// Only for sql server
		// modelBuilder.UseCollation("Turkish_CI_AS");

		modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
	}

	public virtual DbSet<MyEntity> MyEntites => Set<MyEntity>();
}
```

Register DbContext:

```cs
// In this type of service registration, connection string is NOT provided to DI.
// It must be provided in DbContext's OnConfiguring method.
builder.Services.AddDbContext<AppDbContext>();

// In this type of service registration, connection string is provided to DI.
// If DbContext is created by DI, connection string is present in the instance.
// But if user manually creates DbContext, the connection string must also be provided in DbContext's OnConfiguring method
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString: ""));

// Manually            
builder.Services.AddScoped(x => { return new AppDbContext(); });
```

## Migrations

EF Core Migrations is a feature of EF Core that enables developers to evolve the database schema over time in a versioned manner as the application evolves.

With migrations, you can easily apply or revert database changes as your application evolves, without losing existing data.

EF Core Migrations automatically generates and executes the necessary SQL scripts to update the database schema, so you don't have to write manual scripts or worry about data loss.

Command Line | Description
dotnet ef migrations add [name] | Create a new migration with the specific migration name.
dotnet ef migrations remove | Remove the latest migration.
dotnet ef database update | Update the database to the latest migration.
dotnet ef database update [name] | Update the database to a specific migration name point.
dotnet ef migrations list | Lists all available migrations.
dotnet ef migrations script | Generates a SQL script for all migrations.
dotnet ef migrations has-pending-model-changes | Check if there is any model changes since the last migration.
dotnet ef database drop | Drop the database.

### Adding a migration

```zsh
% dotnet ef migrations add Mig0  -c ProjectHavingDbContext -s StartupProject -o Migrations/Folder
% dotnet ef migrations add Mig0 -o App_Data/Migrations
```

### Update Database

```zsh
% dotnet ef database update -c ProjectHavingDbContext -s StartupProject
% dotnet ef database update
```

### Create Migration Scripts

```bash
% dotnet ef migrations script -o script.sql
% dotnet ef migrations script Mig0 Mig1 -o script.sql -c ProjectHavingDbContext -s StartupProject
% dotnet ef migrations script --help
```

### Drop Database

```zsh
% dotnet ef database drop
```

### Remove Migrations

```zsh
% dotnet ef migrations remove
```

## Connection String in Configuration

Add the following section to application configuration (apsettings.json):

```cs
{
  "ConnectionStrings": {
    "DefaultConnection": "provider-specific-connection-string"
  }
}
```

# EF Core Inheritance

```csharp
using Microsoft.EntityFrameworkCore.Relational;

// TPC is supported EF Core 7 and newer versions!
modelBuilder.Entity<EntityName>().UseTpcMappingStrategy();
modelBuilder.Entity<EntityName>().UseTpcMappingStrategy().ToTable(nameof(EntityName));

// For versions EF Core 7 & below:
modelBuilder.Entity<EntityName>().ToTable(nameof(EntityName));
```

# References

* https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration
* https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
* https://www.learnentityframeworkcore.com/inheritance
* https://www.learnentityframeworkcore.com/
* https://www.entityframeworktutorial.net/