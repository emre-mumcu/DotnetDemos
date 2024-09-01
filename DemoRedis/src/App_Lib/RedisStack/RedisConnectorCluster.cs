using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace src.App_Lib.RedisStack
{
    // When you deploy your application, use TLS and follow the Redis security guidelines.
    public sealed class RedisConnectorCluster
    {
        private RedisConnectorCluster() { }

        private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            ConfigurationOptions options = new ConfigurationOptions
            {
                // TODO: Redis endpoints
                EndPoints = {
                    { "localhost", 6379 },
                    { "localhost", 6380 },
                },
            };

            return ConnectionMultiplexer.Connect(options);
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