# MinIO

MinIO is an object storage solution that provides an Amazon Web Services S3-compatible API and supports all core S3 features. MinIO is built to deploy anywhere - public or private cloud, baremetal infrastructure, orchestrated environments, and edge infrastructure.

This procedure deploys a Single-Node Single-Drive MinIO server over https onto Docker.

## Create a Self-Signed Certificate

`mkcert` is a simple tool for making locally-trusted development certificates. It requires no configuration.

Installation (MacOS):

```zsh
% brew install mkcert
% brew install nss # if you use Firefox
```

After installing mkcert, create and install a local CA in the system root store using the following command:

```zsh
% mkcert -install
# Created a new local CA 💥
# The local CA is now installed in the system trust store! ⚡️
# The local CA is now installed in the Firefox trust store (requires browser restart)! 🦊
```

After creating and installing a local CA in the system root store, generates locally-trusted certificates:

```zsh
% mkcert -cert-file public.crt -key-file private.key localhost 127.0.0.1
```

NOTE: certificate names must be public.crt and private.key for minio to auto discover them.

## Create Volumes

Create the following folders in host to use as persistance volumnes in minio container:

```zsh
% mkdir -p /Users/emre/DockerVol/minio/data
% mkdir -p /Users/emre/DockerVol/minio/ssl
```

Copy both public.crt and private.key files to the host `/Users/emre/DockerVol/minio/ssl` path.

Create the minio container using the following docker command:

```zsh
% docker run -d \
   -p 9000:9000 \
   -p 9001:9001 \
   --hostname localhost \
   --name minio \
   -v /Users/emre/DockerVol/minio/data:/data \
   -v /Users/emre/DockerVol/minio/ssl:/opt/minio/certs \
   -e "MINIO_ROOT_USER=root" \
   -e "MINIO_ROOT_PASSWORD=root" \
   quay.io/minio/minio server /data --console-address ":9001" --certs-dir "/opt/minio/certs"
```

After creating he container, you can access the MinIO Console by navigating to https://127.0.0.1:9001. While port 9000 is used for connecting to the API, MinIO automatically redirects browser access to the MinIO Console (9001).

Log in to the Console with the credentials you defined in the MINIO_ROOT_USER and MINIO_ROOT_PASSWORD environment variables.

# MinIO Client SDK for .NET

MinIO Client SDK provides higher level APIs for MinIO and Amazon S3 compatible cloud storage services.

```zsh
% dotnet add package minio
```

# MinIO Client Sample for ASP.NET

Create the following Minio configuration in appsettings.json:

```json
{
  "Minio": {
	"Endpoint": "127.0.0.1:9000",
	"AccessKey": "UStBVq28XkzEiNkBLcdL",
	"SecretKey": "JFih0tREGrz1rdOSMj3PLIroVMD2wG5AN3rdL6O9",
	"BucketName": "default"
  }
}
```

Create the following proxy class for Minio configuration:

```cs
public class MinioConfig
{
	public string Endpoint { get; set; } = null!;
	public string AccessKey { get; set; } = null!;
	public string SecretKey { get; set; } = null!;
	public string BucketName { get; set; } = null!;
}
```

Register the Minio service:

```cs
using Minio;

builder.Services.AddMinio(configureClient => {

	var mc = builder.Configuration.GetSection("Minio").Get<MinioConfig>();

	configureClient
		.WithEndpoint(mc!.Endpoint)
		.WithCredentials(mc!.AccessKey, mc!.SecretKey)
		.WithSSL()
		.Build();
	}
);
```

# References

* https://min.io/docs/minio/container/index.html
* https://min.io/docs/minio/linux/developers/dotnet/minio-dotnet.html
* https://min.io/docs/minio/linux/developers/dotnet/API.html
* https://min.io/docs/minio/container/operations/network-encryption.html
* https://github.com/FiloSottile/mkcert