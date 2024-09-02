using Microsoft.Extensions.Logging;

namespace src.App_Lib;

public class CustomLogger(LoggingDbContext DBContext, string CategoryName) : ILogger
{
	// BeginScope develops a new logging scope. 
	// A logging scope collects related log messages and links them to a particular operation or context.
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		// return null;
		return default;
	}

	// IsEnabled verifies that a specified log level can write log messages. 
	// This method aids in optimizing code by bypassing the overhead of formatting and writing invalid log messages.
	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel != LogLevel.None;
	}

	// Log is the main logging method in .NET. 
	// This method writes a log message with a specified log level, event ID and state.
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}
		
		var message = formatter(state, exception);
		var threadId = Thread.CurrentThread.ManagedThreadId;
		var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel}] {CategoryName} - {threadId} - {message}";

		var logEntry = new LogEntry
		{
			LogLevel = logLevel.ToString(),
			Category = CategoryName,
			Message = logMessage,
			Timestamp = DateTime.UtcNow
		};

		// Save log entry to database
		Task.Run(() => DBContext.LogEntries.Add(logEntry)).Wait();
		Task.Run(() => DBContext.SaveChanges()).Wait();
	}
}
