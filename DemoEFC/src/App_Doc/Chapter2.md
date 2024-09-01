# Chapter-2: EFCore Code-First


```bash
# https://stackoverflow.com/questions/48341164/how-to-make-postgres-docker-accessible-for-any-ip-remotely
You have to create postgresql.conf whith parameters, and set listen_addresses = '*'

attach when starting your container.

docker run -p 5432:5432 -e POSTGRES_PASSWORD=123456789 \
 -d postgres:9.3.6 \
 -c config_file=/path/to/postgresql.conf

 ##

    > docker exec -it POSTGRES_DB bash
    > apt-get update
    > apt-get install vim
    # psql -U postgres -c 'SHOW config_file'
    /var/lib/postgresql/data/postgresql.conf

```


dotnet ef migrations add Mig0 -o App_Data/Migrations

dotnet ef migrations list

# Adding a migration

```bash
# Open the terminal for the project having DbContext
# dotnet ef migrations add Mig0  [-c ProjectHavingDbContext] [-s StartupProject] [-o Migrations/Folder]
> dotnet ef migrations add Mig0 -o App_Data\Migrations
```

# Update Database

```bash
# dotnet ef database update  [-c ProjectHavingDbContext] [-s StartupProject] 
> dotnet ef database update
```

# Create Migration Scripts

```bash
# dotnet ef migrations script -o script.sql
# dotnet ef migrations script Mig0 Mig1 -o script.sql [-c ProjectHavingDbContext] [-s StartupProject] 
> dotnet ef migrations script --help
```

# EF Core Inheritance

https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
https://www.learnentityframeworkcore.com/inheritance

``` csharp
using Microsoft.EntityFrameworkCore.Relational; 

// TPC sadece EFCore7 ve üzerinde destekleniyor
modelBuilder.Entity<EntityName>().UseTpcMappingStrategy();
modelBuilder.Entity<EntityName>().UseTpcMappingStrategy().ToTable(nameof(EntityName));

// EF Core 7 Öncesinde aşağıdaki gibi çözülebilir ancak inherit edilen propertiy'lerde sorun oluyor!
// modelBuilder.Entity<EntityName>().ToTable(nameof(EntityName));
```


dotnet ef migrations add Mig0 -o App_Data\Migrations
dotnet ef database update
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations script --help

whoami /user
Get-Acl
dotnet watch run

```bash

#Dotnet-ef Tools
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

#Design Component
dotnet add package Microsoft.EntityFrameworkCore.Design

#Providers
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package MySql.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.EntityFrameworkCore.InMemory


dotnet ef database update [-p <ProjectHavingDbContext> -s <StartupProject> -o PathToMigrations]
dotnet ef database drop [-p <ProjectHavingDbContext> -s <StartupProject> ]
dotnet ef migrations remove [-p <ProjectHavingDbContext> -s <StartupProject> ]

#Database First
dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=MyDatabase;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models
dotnet ef dbcontext scaffold "Server=127.0.0.1;Port=5454;Database=mumcu;User Id=postgres;Password=aA123456;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models


#Code First
dotnet ef migrations add Init -o AppLib/Data/Migrations
dotnet ef database update

dotnet ef migrations list

https://www.learnentityframeworkcore.com/
https://www.entityframeworktutorial.net/


```


ServiceProvider serviceProvider = new ServiceCollection()
    .AddDbContext<SampleDbContext>(options => options.UseNpgsql(connectionString: "Server=127.0.0.1;Port=5454;Database=sample;User Id=postgres;Password=aA123456;"))
    .BuildServiceProvider();


//IConnection connection = serviceProvider.GetService<IConnection>();
//ICommand command = serviceProvider.GetService<ICommand>();


    // dotnet add package Newtonsoft.Json
    dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation


















