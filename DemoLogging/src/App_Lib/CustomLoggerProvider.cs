namespace src.App_Lib
{
	public class CustomLoggerProvider : ILoggerProvider
	{
		public ILogger CreateLogger(string categoryName)
		{
			return new CustomLogger(new LoggingDbContext(), categoryName);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}