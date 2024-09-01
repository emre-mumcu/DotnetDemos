using StackExchange.Redis;

namespace src.App_Lib.RedisStack
{
	public sealed class RedisConnector
	{
		private RedisConnector() { }

		private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
		{
			RedisOptions? redisOptions = App.Instance.DataConfiguration.GetSection("Redis").Get<RedisOptions>();

			if(redisOptions == null) throw new ArgumentNullException(nameof(RedisOptions));
			
			else return ConnectionMultiplexer.Connect(redisOptions.Configuration);
		});

		public static ConnectionMultiplexer Connection
		{
			get
			{
				return lazyConnection.Value;
			}
		}
	}
}