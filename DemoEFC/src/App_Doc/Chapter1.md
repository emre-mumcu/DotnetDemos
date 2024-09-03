# Chapter-1: Getting Started

Entity Framework Core (EF Core) is an open-source, lightweight, and cross-platform version of Entity Framework data-access technology which is an Object-Relational Mapper (ORM) tool that enables developers to work with relational databases using .NET objects eliminating the need for most of the data-access code developers typically need to write. 

## Entity Framework Core Tools

The command-line interface (CLI) tools for Entity Framework Core perform design-time development tasks. The commands are an extension to the cross-platform dotnet command.

EFCore tools can be installed as either a global or local tool. To install dotnet ef as a global tool, use the following command:

```zsh
% dotnet tool install --global dotnet-ef
```

Update the EFCore tools using the following command:

```zsh
dotnet tool update --global dotnet-ef
```

Before you can use the tools on a specific project, you'll need to add the Microsoft.EntityFrameworkCore.Design package to it.

```zsh
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Creating a Sample Project

To create a sample web application, open a command prompt and run the following commands:

```zsh
% mkdir DemoEFC
% cd DemoEFC
% dotnet new sln
% dotnet new mvc -o src
% dotnet sln add src/.
```

## 2. Entity Framework Core Setup

EF Core is available as a Nuget Package that can be added to your project. 

To install EF Core, you install the package for the EF Core database provider(s) you want to target. Run one of the following commands in the terminal window:

```zsh
% dotnet add package Microsoft.EntityFrameworkCore.SqlServer
% dotnet add package Microsoft.EntityFrameworkCore.Sqlite
% dotnet add package Microsoft.EntityFrameworkCore.InMemory
% dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
% dotnet add package Pomelo.EntityFrameworkCore.MySql
% dotnet add package Oracle.EntityFrameworkCore
```

# References

* https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
* https://www.learnentityframeworkcore.com
* https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx
* https://dotnettutorials.net/lesson/entity-framework-core
* https://www.csharptutorial.net/entity-framework-core-tutorial
* https://learn.microsoft.com/en-us/ef/core/cli/dotnet
* https://stackify.com/entity-framework-core-tutorial
* https://www.tektutorialshub.com/entity-framework-core-tutorial