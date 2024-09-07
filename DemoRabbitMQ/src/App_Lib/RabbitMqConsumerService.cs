using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace src.App_Lib
{
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
}