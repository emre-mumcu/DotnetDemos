dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
dotnet add package Newtonsoft.Json
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Markdig
dotnet add package BouncyCastle.Cryptography

https://learn.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-8.0
dotnet-aspnet-codegenerator
---------------------------

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

dotnet tool install --global dotnet-aspnet-codegenerator

***

dotnet aspnet-codegenerator --help
----
-p|--project             Path to .csproj file in the project.
--no-build


dotnet aspnet-codegenerator controller --help
----
--useAsyncActions|-async   
--restWithNoViews|-api   

examples:
cd *.csproj folder
Create MVC Controller
dotnet aspnet-codegenerator -p . controller -name DemoController -outDir .\Controllers

Create WebApi Controller
dotnet aspnet-codegenerator -p . controller -name DemoController -outDir .\Controllers -api

dotnet aspnet-codegenerator controller -name DataController -api -outDir .\Controllers



dotnet aspnet-codegenerator --project . controller -name DemoController --output .

dotnet aspnet-codegenerator --project . controller -name DemoController  -m Author -dc WebAPIDataContext
