# 1. Cretae Project

dotnet new mvc -o src
dotnet new sln
dotnet sln add src/.
dotnet restore

If you want to use dotnet development certificate just run the following code:

```zsh
% dotnet dev-certs https --trust
```

## 2. Edit launchsettings.json to use Https

Edit the launchsettings.json file as follows:

```json
{
	"$schema": "https://json.schemastore.org/launchsettings.json",
	"profiles": {
		"kestrel": {
			"commandName": "Project",
			"dotnetRunMessages": true,
			"applicationUrl": "https://localhost:5001;http://localhost:5000",
			"environmentVariables": {
				"ASPNETCORE_ENVIRONMENT": "Development"
			}
		}
	}
}
```

# 3. Create a Self-Signed Certificate

## 3.1. p12/pfx Certificate

```zsh
# Alternative 1
# Create p12/pfx certificate using mkcert
# Note: mkcert created certificate is trusted by macos since mkcert add CA to KeyChain when 'mkcert -install' is run
% mkcert -pkcs12 localhost

# Alternative 2
# Create p12/pfx certificate using openssl
% openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout key.pem -out localhost.pem -days 365
% openssl pkcs12 -inkey key.pem -in localhost.pem -export -out localhost.p12

# View Certificate
% openssl pkcs12 -in localhost.p12 -noout -info
```

## 3.2. pem/crt Certificate

```zsh
# Alternative 1
# Create pem certificate using mkcert
# Note: mkcert created certificate is trusted by macos since mkcert add CA to KeyChain when 'mkcert -install' is run
% mkcert localhost

# Alternative 2
# Create pem certificate using openssl
% openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout key.pem -out localhost.pem -days 365

# View Certificate
% openssl -in localhost.pem -noout -info
```

# 4. Use Certificate

## 4.1. Use Certificate in Configuration

### 4.1.1. Use p12/pfx Certificate in Configuration

Create a kestrel.p12.json file to include Kestrel configuration parameters:

```json
{
	"Kestrel": {
		"Certificates": {
			"Default": {
				"Path": "ssl/localhost.p12",
				"Password": "changeit"
			}
		}
	}
}
```

Include kestrel.p12.json file to the configuration:

```cs
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"kestrel.p12.json", optional: false, reloadOnChange: false);
```

### 4.1.2. Use pem/crt Certificate in Configuration

NOTE: Unable to make it run
Unhandled exception. System.ArgumentException: No supported key formats were found. Check that the input represents the contents of a PEM-encoded key file, not the path to such a file. (Parameter 'input')

Create a kestrel.pem.json file to include Kestrel configuration parameters:

```json
{
  "Kestrel": {
    "Certificates": {
      "Default": {
        "Path": "ssl/localhost.pem",
        "KeyPath": "ssl/localhost-key.pem",
        "Password": "$CREDENTIAL_PLACEHOLDER$"
      }
    }
  }
}
```

Include kestrel.pem.json file to the configuration:

```cs
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"kestrel.pem.json", optional: false, reloadOnChange: false);
```

## 4.2. Use Certificate in Code

```cs
builder.WebHost.ConfigureKestrel(options =>
{
	// Listen on port 5000 for HTTP
	options.ListenAnyIP(5000);

	// Listen on port 5001 for HTTPS
	options.ListenAnyIP(5001, listenOptions =>
	{

		// Use default dotnet development certificate
		// listenOptions.UseHttps();

		// Use custom certificate
		listenOptions.UseHttps("certificate.pfx", "12345678");
	});

	options.Limits.MaxConcurrentConnections = 100;

	options.Limits.MaxRequestBodySize = 10 * 1024; // 10 KB
});
```

```cs
// Read Certificate [optional]
X509Certificate2 cert = new X509Certificate2("certificate.pfx", "12345678");

// Configure Kestrel using settings section
builder.WebHost.ConfigureKestrel((context, options) =>
{
	var kestrelConfig = context.Configuration.GetSection("Kestrel");
	options.Configure(kestrelConfig);
});
```

## Alternative Kestrel configurations:

```json
{
	"Kestrel": {
		"Certificates": {
			"Default": {
				"Path": "certificate.pfx",
				"Password": "12345678"
			}
		},
		"Endpoints": {
			"Http": {
				"Url": "http://*:5000"
			},
			"Https": {
				"Url": "https://*:5001",
				"Certificate": {
					"Path": "certificate.pfx",
					"KeyPassword": "12345678"
				}
			}
		},
		"Limits": {
			"MaxConcurrentConnections": 100,
			"MaxRequestBodySize": 10485760
		}
	}
}

{
	"Kestrel": {
		"Endpoints": {
			"HttpsFromPem": {
				"Url": "https://localhost:5001",
				"Certificate": {
					"Path": "ssl/localhost.pem",
					"KeyPath": "ssl/localhost-key.pem"
				}
			}
		}
	}
}
```