using System.Text;
using RabbitMQ.Client;

namespace src.App_Lib
{
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
}