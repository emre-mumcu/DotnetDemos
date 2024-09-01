# Docker

docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest

# Packages

dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
dotnet add package StackExchange.Redis
dotnet add package NRedisStack

**Note**
1) If you use NRedisStack instead of StackExchange.Redis, you will be leveraging additional Redis modules like RediSearch, RedisGraph, RedisJSON, and others that NRedisStack provides support for.
2) To secure Redis, use TLS and follow the Redis security guidelines.
3) To convert user certificate and private key from the PEM format to pfx, use this command: 
```zsh
% openssl pkcs12 -inkey redis_user_private.key -in redis_user.crt -export -out redis.pfx
```

When you deploy your application, use TLS and follow the Redis security guidelines.

# Using Redis for Distributed User Sessions in ASP.NET Core

https://requestmetrics.com/building/episode-16-using-redis-for-distributed-user-sessions-in-asp-net-core

We need distributed session state because load balancing with sticky sessions is whack.

ASP.NET session storage is useful for storing state across page views. In single server situations it’s simple to set up because ASP.NET supports in-memory session out of the box. In-memory sessions stop working as soon as there is more than one server. Most production environments have more than one server so the session issue needs to be dealt with.

There are two options for sessions in a web farm. First, a load balancer can be used to lock each user on a specific box (so-called “sticky sessions” or “session affinity”). This lets us continue to use in-memory session. The second is switching from in-memory to distributed session storage.

We already use StackExchange.Redis for talking to Redis. Conveniently, Microsoft has a ready-made distributed cache package which uses StackExchange.Redis. The first step is to install it:

```zsh
% dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

The application must be configured to use the StackExchangeRedis cache. Be sure to tell the cache which Redis instance to connect to:

```cs
// Configure Redis Based Distributed Session
var redisConfigurationOptions = ConfigurationOptions.Parse("localhost:6379");

services.AddStackExchangeRedisCache(redisCacheConfig => {
	redisCacheConfig.ConfigurationOptions = redisConfigurationOptions;
});

services.AddSession(options => {
	options.Cookie.Name = "myapp_session";
	options.IdleTimeout = TimeSpan.FromMinutes(60 * 24);
});

// Use session
app.UseSession();
```

With the Redis cache configured, reading and writing to the user’s session should work:

```cs
var value = $"Session written at {DateTime.UtcNow.ToString()}";
HttpContext.Session.SetString("Test", value);
var value = HttpContext.Session.GetString("Test");
```

https://codewithmukesh.com/blog/distributed-caching-in-aspnet-core-with-redis/



You can use launchSettings.json to configure how your application is launched in development, including specifying which URLs Kestrel should listen on. However, launchSettings.json primarily affects the development environment and is not used in production scenarios.

# References

https://redis.io/docs/latest/develop/interact/search-and-query/query/
https://csharpindepth.com/Articles/Singleton 
https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-8.0
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0