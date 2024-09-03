using Microsoft.EntityFrameworkCore;

namespace src.App_Lib
{
	public class AppDbContext : DbContext
	{
		private readonly IServiceProvider? _serviceProvider;

		public AppDbContext() { }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider) : base(options)
		{
			_serviceProvider = serviceProvider;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

			base.OnConfiguring(optionsBuilder);

			if (!optionsBuilder.IsConfigured)
			{
				var config = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("data.json", optional: false)
					.Build();

				optionsBuilder.UseSqlite(connectionString: config.GetConnectionString("DefaultConnection"));
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
		}

		// public virtual DbSet<EntityObj> Entities => Set<EntityObj>();
	}
}