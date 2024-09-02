using Microsoft.Extensions.Logging;

namespace src.App_Lib
{
	public class CustomLoggerProvider : ILoggerProvider
	{
		private readonly LoggingDbContext _dbContext;

		public CustomLoggerProvider(LoggingDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new CustomLogger(_dbContext, categoryName);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}