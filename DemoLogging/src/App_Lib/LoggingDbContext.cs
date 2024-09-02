using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace src.App_Lib
{
	public class LoggingDbContext : DbContext
	{
		public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

		public DbSet<LogEntry> LogEntries { get; set; }
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