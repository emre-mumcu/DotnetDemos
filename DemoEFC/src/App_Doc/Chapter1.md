# Chapter-1: Getting Started

Entity Framework Core (EF Core) is an open-source, lightweight, and cross-platform version of Entity Framework data-access technology which is an Object-Relational Mapper (ORM) tool that enables developers to work with relational databases using .NET objects. 

## 1. Creating a Sample Project

This tutorial assumes you have already installed a valid .NET SDK and you already have an IDE of your choice. 
First of all, we will create a sample web application. For this, open a command prompt and run the following commands to create a new application. Do not forget to change the folder names according to your own preference:

``` bash
C:\Tutorials> mkdir DemoEFC
C:\Tutorials> cd DemoEFC
C:\Tutorials\EFCoreDemo> dotnet new sln
C:\Tutorials\EFCoreDemo> dotnet new mvc -o src
C:\Tutorials\EFCoreDemo> dotnet sln add src/.

# For Visual Studio: Double click the DemoEFC.sln to open the project
# For Visual Studio Code: Right click the DemoEFC folder and and select Open with VSCode (or type 'code .' in command prompt in DemoEFC folder)
```

## 2. Entity Framework Core Setup

EF Core is available as a Nuget Package that can be added to your project. In this tutorial we will be using SqlServer.

2.1. To install EF Core, you install the package for the EF Core database provider(s) you want to target. Right click the project name (EFCoreDemo) in Visual Studio and select `Open in Terminal` option. Run the following command in the terminal window:

``` bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

2.2. You also need the Entity Framework Core tools if you want to make use of EF commands for migrations, scaffolding, etc in the Package Manager Console (PMC). The Entity Framework Core tools help with design-time development tasks. They're primarily used to manage Migrations and to scaffold a DbContext and entity types by reverse engineering the schema of a database. 

Either of the following tools can be installed, as both tools expose the same functionality:

2.2.A. The EF Core Package Manager Console (PMC) tools run in the Package Manager Console in Visual Studio. Run the following command to install PMC tools:

``` bash
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

2.2.B. The EF Core .NET command-line interface (CLI) tools are an extension to the cross-platform .NET Core CLI tools. These tools require a .NET Core SDK project. This is a global installation of efcore tools and you need to do it once. Once installed you may try to update the tools from time to time to get the latest version. Run the following command to install EF Core .NET CLI tools:

``` bash
# install
dotnet tool install --global dotnet-ef
# update
dotnet tool update --global dotnet-ef
```

3) Before you can use the EF Core tools on a specific project, you'll need to add the Microsoft.EntityFrameworkCore.Design package to it. Microsoft.EntityFrameworkCore.Design is a DevelopmentDependency package. Microsoft.EntityFrameworkCore.Design contains all the design-time logic for Entity Framework Core.

``` bash
dotnet add package Microsoft.EntityFrameworkCore.Design
```

You can check and verify the installed packages in the project by running the following command:

``` bash
dotnet list package
``` 

Now you are ready to use EF Core on your project.

# References

* <a href="https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli" target="_blank">https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli</a>
* <a href="https://www.learnentityframeworkcore.com/" target="_blank">https://www.learnentityframeworkcore.com/</a>
* <a href="https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx" target="_blank">https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx</a>
* <a href="https://dotnettutorials.net/lesson/entity-framework-core/" target="_blank">https://dotnettutorials.net/lesson/entity-framework-core/</a>
* <a href="https://www.csharptutorial.net/entity-framework-core-tutorial/" target="_blank">https://www.csharptutorial.net/entity-framework-core-tutorial/</a>
* <a href="https://learn.microsoft.com/en-us/ef/core/cli/dotnet" target="_blank">https://learn.microsoft.com/en-us/ef/core/cli/dotnet</a>