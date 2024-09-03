using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace src.App_Lib
{
	/// <summary>
	/// If a class implementing Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<TContext> interface is found 
	/// in either the same project as the derived DbContext or in the application's startup project, 
	/// the tools bypass the other ways of creating the DbContext and use the design-time factory instead.
	/// </summary>
	public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
	{
		// Both IDesignTimeDbContextFactory<TContext>.CreateDbContext and Program.CreateHostBuilder accept command line arguments.
		public AppDbContext CreateDbContext(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("data.json", optional: false)
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

			optionsBuilder.UseSqlite(config.GetConnectionString("DefaultConnection"));

			return new AppDbContext(optionsBuilder.Options);
		}
	}
}