# Chapter-3: EFCore DB-First

# DB First

https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli

dotnet user-secrets init

dotnet user-secrets set ConnectionStrings:Chinook "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook"

dotnet ef dbcontext scaffold "Name=ConnectionStrings:Chinook" Microsoft.EntityFrameworkCore.SqlServer

dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer

-Context AppDbContext --context-dir Data --output-dir Models