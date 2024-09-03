# Chapter-3: DB-First

Reverse engineering is the process of scaffolding entity type classes and a DbContext class based on a database schema. EF Core does not support visual designer for DB model and wizard to create the entity and context classes similar to EF so, we need to do reverse engineering using the `dotnet ef dbcontext scaffold`command. This reverse engineering command creates entity and context classes (by deriving DbContext) based on the schema of the existing database.


`dotnet ef dbcontext scaffold`command has two required arguments: the connection string to the database, and the EF Core database provider to use.

```zsh
% dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

## User secrets for connection strings

If you have a .NET application that uses the hosting model and configuration system, such as an ASP.NET Core project, then you can use the `Name=<connection-string>` syntax to read the connection string from configuration.

```json
{
  "ConnectionStrings": {
    "Chinook": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Chinook"
  }
}
```

This connection string in the config file can be used to scaffold from a database using:

```zsh
% dotnet ef dbcontext scaffold "Name=ConnectionStrings:Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

However, storing connection strings in configuration files is not a good idea, since it is too easy to accidentally expose them, for example, by pushing to source control. Instead, connection strings should be stored in a secure way, such as using Azure Key Vault or, when working locally, the Secret Manager tool, aka "User Secrets".

For example, to use the User Secrets, first remove the connection string from your ASP.NET Core configuration file. Next, initialize User Secrets by executing the following command in the same directory as the ASP.NET Core project:

```zsh
% dotnet user-secrets init
```

This command sets up storage on your computer separate from your source code and adds a key for this storage to the project.

Next, store the connection string in user secrets. For example:

```zsh
% dotnet user-secrets set ConnectionStrings:Chinook "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook"
```

Now the same command that previous used the named connection string from the config file will instead use the connection string stored in User Secrets. For example:

```zsh
dotnet ef dbcontext scaffold "Name=ConnectionStrings:Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

By default, the scaffolder will include the connection string in the scaffolded code, but with a warning.

The --no-onconfiguring option can be passed to suppress creation of the OnConfiguring method containing the connection string.

# References

https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli
