# Docker

docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest

dotnet dev-certs https --trust

mkcert localhost

openssl pkcs12 -export -out localhost.pfx -inkey localhost-key.pem -in localhost.pem 

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

You can use launchSettings.json to configure how your application is launched in development, including specifying which URLs Kestrel should listen on. However, launchSettings.json primarily affects the development environment and is not used in production scenarios.

# References

https://redis.io/docs/latest/develop/interact/search-and-query/query/
https://csharpindepth.com/Articles/Singleton 
https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-8.0
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0