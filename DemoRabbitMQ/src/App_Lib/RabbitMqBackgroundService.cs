namespace src.App_Lib;

public class RabbitMqBackgroundService : BackgroundService
{
	private readonly RabbitMqConsumerService _consumerService;

	public RabbitMqBackgroundService(RabbitMqConsumerService consumerService)
	{
		_consumerService = consumerService;
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_consumerService.StartConsuming(message =>
		{
			// Handle the received message
			Console.WriteLine($"Received message: {message}");
		});

		// Keep the service running
		return Task.CompletedTask;
	}
}
