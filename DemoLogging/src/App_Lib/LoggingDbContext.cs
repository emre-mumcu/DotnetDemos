using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace src.App_Lib
{
	public class LoggingDbContext : DbContext
	{
		public LoggingDbContext() { }
		public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

			base.OnConfiguring(optionsBuilder);

			if (!optionsBuilder.IsConfigured)
			{
				string? connectionString = App.Instance.DataConfiguration.GetConnectionString("DefaultConnection");
				optionsBuilder.UseSqlite(connectionString: connectionString!);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

			// modelBuilder.SeedData();
		}

		public virtual DbSet<LogEntry> LogEntries => Set<LogEntry>();
	}

	public class LogEntry
	{
		public int Id { get; set; }
		public string LogLevel { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string Message { get; set; } = null!;
		public DateTime Timestamp { get; set; }
	}
}