# RabbitMQ

RabbitMQ is a reliable and mature messaging and streaming broker, which is easy to deploy on cloud environments, on-premises, and on your local machine.

To install rabbitmq with management plugin preinstalled, use the following docker command:

```zsh
% docker run -d --name rabbitmq -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

Open a web browser and navigate to http://localhost:15672 to check out the management web app. Log in using the default username `guest` and password `guest`.

## Sample Application

RabbitMQ speaks multiple protocols. This tutorial uses AMQP 0-9-1, which is an open, general-purpose protocol for messaging.

Ad the following package:

```zsh
% dotnet add package RabbitMQ.Client
```

### Message Sender Service

```cs
using System.Text;
using RabbitMQ.Client;

public class RabbitMqSenderService
{
	private readonly IConnection _connection;
	private readonly IModel _channel;

	public RabbitMqSenderService(string hostname, string username, string password)
	{
		var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
	}

	public void SendMessage(string queueName, string message)
	{
		var body = Encoding.UTF8.GetBytes(message);

		_channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
		
		_channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
	}
}
```

### Message Receiver Service

```cs
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMqConsumerService
{
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _queueName;

	public RabbitMqConsumerService(string hostname, string username, string password, string queueName)
	{
		_queueName = queueName;

		var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
		
		_connection = factory.CreateConnection();
		
		_channel = _connection.CreateModel();
		
		_channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
	}

	public void StartConsuming(Action<string> messageHandler)
	{
		var consumer = new EventingBasicConsumer(_channel);

		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			messageHandler(message);
		};
		
		_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
	}
}
```

### Register Receiver Service as Background Service

```cs
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
```

### Create RabbitMQ Configuration Section

Edit the appsettings.json as follows:

```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"AllowedHosts": "*",
	"RabbitMQ": {
		"HostName": "localhost",
		"UserName": "guest",
		"Password": "guest"
	}
}
```

### Register Services in Configuration

Register all services to the builder:

```cs
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

builder.Services.AddSingleton(new RabbitMqSenderService(
	rabbitMqConfig["HostName"]!,
	rabbitMqConfig["UserName"]!,
	rabbitMqConfig["Password"]!
));

builder.Services.AddSingleton(new RabbitMqConsumerService(
	rabbitMqConfig["HostName"]!,
	rabbitMqConfig["UserName"]!,
	rabbitMqConfig["Password"]!,
	"test_queue" // Replace with your queue name
));

builder.Services.AddHostedService<RabbitMqBackgroundService>();
```

### Create a message sender form:

```html
<div class="text-center">
	<h1 class="display-4">Send Message to RabbitMQ</h1>
	<form asp-action="SendMessage" method="post">
		<div class="form-group">
			<label for="message">Message</label>
			<input type="text" class="form-control" id="message" name="message" required />
		</div>
		<button type="submit" class="btn btn-primary">Send</button>
	</form>
</div>
```

```cs
public class HomeController(RabbitMqSenderService rabbitMqService) : Controller
{
	[HttpPost]
	public IActionResult SendMessage(string message)
	{
		rabbitMqService.SendMessage("test_queue", message);
		return RedirectToAction("Index");
	}
}
```

Run the application and send some messages!