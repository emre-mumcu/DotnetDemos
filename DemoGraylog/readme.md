# 1. Graylog Setup

## 1.1. Using docker Command

Create MongoDB, ElasticSearch and Graylog containers and link them together.

NOTE: ElasticSearch wil NOT be supported anymore and opensearch is recommended by Graylog. But, by the time of creating this document I was unable to make it work for the graylog. So I use ElasticSearch instead.

```zsh
# MongoDB
% docker run --name mongo -d mongo:3

# ElasticSearch
% docker run --name elasticsearch -e "http.host=0.0.0.0" -e "xpack.security.enabled=false" -d docker.elastic.co/elasticsearch/elasticsearch-oss:6.5.4

# Mongo ve elasticsearch ü graylog a linkleyerek ayağa kaldıracağız.
% docker run --link mongo --link elasticsearch -p 9000:9000 -p 12201:12201 -p 514:514 -e GRAYLOG_WEB_ENDPOINT_URI="http://127.0.0.1:9000/api" -d graylog/graylog:2.5
```

Default username and password is "admin". To change the password, fir create a SHA256 for the password:

```zsh
% echo -n mypass | sha256sum
```

Then recreate the graylog container using new password:

```zsh
docker run --link mongo --link elasticsearch -p 9000:9000 -p 12201:12201 -p 514:514 -e GRAYLOG_WEB_ENDPOINT_URI="http://127.0.0.1:9000/api" -e GRAYLOG_ROOT_PASSWORD_SHA2=xxx -d graylog/graylog:2.5
```

## 1.2. Using docker compose Command

Create the following docker compose file `graylog-stack.yml`:

```yml
version: '2'
services:
  mongodb:
    image: mongo:3
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch-oss:6.5.4
    environment:
      - http.host=0.0.0.0
      - transport.host=localhost
      - network.host=0.0.0.0
      - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
    ulimits:
      memlock:
        soft: -1
        hard: -1
    mem_limit: 1g
  graylog:
    image: graylog/graylog:2.5
    environment:
      - GRAYLOG_PASSWORD_SECRET=somepasswordpepper
      # Password: admin
      - GRAYLOG_ROOT_PASSWORD_SHA2=8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918
      - GRAYLOG_WEB_ENDPOINT_URI=http://127.0.0.1:9000/api
    links:
      - mongodb:mongo
      - elasticsearch
    depends_on:
      - mongodb
      - elasticsearch
    ports:
      # Graylog web interface and REST API
      - 9000:9000
      # Syslog TCP
      - 514:514
      # Syslog UDP
      - 514:514/udp
      # GELF TCP
      - 12201:12201
      # GELF UDP
      - 12201:12201/udp
```

```zsh
# check file is valid
% docker compose -f graylog-stack.yml config

# pull images and create containers (detached mode)
% docker compose -f graylog-stack.yml up -d
```

# 2. Graylog Configuration

## 2.1. Input

Open Graylog (http://localhost:9000) and login using admin user.

* Click System -> Inputs
* From the "Select Input" dropdown, select GELF UDP
* Click "Launch new input"
* Click Global checkbox
* Set a Title for input
* Set Bind Address as 0.0.0.0 for any IP address or set a specified IP address.
* Set Port or leave it as 12201
* Click Save

### 2.2. Output

Open Graylog (http://localhost:9000) and login using admin user.

* Click System -> Outputs
* From the "Select Output Type" dropdown, select GELF Output
* Click "Launch new output"
* Set a Title for output
* Set Bind Address the same as the Input Bind Address (0.0.0.0 or the IP that you specified in previous step)
* Update Port if you changed it in previous step or leave it as 12201
* Set UDP as Protocol
* Click Save

Graylog is ready for use.

# 3. Using Graylog in ASP.NET Core Application

Add the following package:

```zsh
% dotnet add package Gelf.Extensions.Logging
```
Configure appsettings.json or any custom configuration you will add to the builder.Configuration

NOTE: Gelf.Extensions.Logging package will not make use of auto use the configuration. You need to specify the Gelf options properties manually in configuration (AddGelf).

```json
{
    "GELF": {
        "Host": "localhost",
        "Port": 12201, // Not required if using default 12201.
        "Protocol": "UDP", // Not required if using default UDP.
        "LogSource": "My.App.Name", // Not required if set in code as above.
        "AdditionalFields": { // Optional fields added to all logs. But json is not parsed automatically by provider so this can be done in code too.
            "instance-guid": "A7AE7017-4CAE-43DB-92F9-B35BB29787D8",   // Optional dynamic field name-valu
            "version": "1.0.0.0 (b1)"  // Optional dynamic field name-value
        },
        "LogLevel": {
            "Default": "Debug",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
        }
    }
}
```

Create a proxy class for GELF options

```cs
public class GelfConfiguration
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 12201;
    public GelfProtocol Protocol { get; set; }  = GelfProtocol.Udp;
    public string? LogSource { get; set; } = Assembly.GetExecutingAssembly().GetName().Name;
}
```

Update the services to use Gelf

```cs
// If you have GELF configuration in a custom.json file instead of appsettings.json file
builder.Configuration.AddJsonFile($"custom.json", optional: false, reloadOnChange: false);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Logging.AddGelf(options => {
    var configuration = builder.Configuration;
    var gc = builder.Configuration.GetSection("GELF").Get<GelfConfiguration>();
    options.Host = gc!.Host;
    options.Port = gc!.Port;
    options.Protocol =  gc!.Protocol;
    options.LogSource = gc.LogSource;
    options.AdditionalFields["instance-guid"] = Guid.NewGuid().ToString();
});
```

Make some dummy logging and check if you have them in Graylog.

```cs
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogTrace($"LogTrace {LogLevel.Trace}"); // 0
        _logger.LogDebug($"LogDebug {LogLevel.Debug}"); // 1
        _logger.LogInformation($"LogInformation {LogLevel.Information}"); // 2
        _logger.LogWarning($"LogWarning {LogLevel.Warning}"); // 3
        _logger.LogError($"LogError {LogLevel.Error}"); // 4
        _logger.LogCritical ($"LogCritical {LogLevel.Critical}"); // 5
        _logger.Log(LogLevel.None, "NONE"); // 6        

        return View();
    }       
}
```

# References

* https://blog.barisceviz.com/asp-net-core-graylog-log-monitoring-entegrasyonu-da5962fc66aa
* https://github.com/mattwcole/gelf-extensions-logging