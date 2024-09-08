// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-6.0#attribute-routing-for-rest-apis
// https://docs.min.io/docs/dotnet-client-quickstart-guide.html
// https://docs.min.io/docs/dotnet-client-api-reference.html

# MinIO

MinIO is an object storage solution that provides an Amazon Web Services S3-compatible API and supports all core S3 features. MinIO is built to deploy anywhere - public or private cloud, baremetal infrastructure, orchestrated environments, and edge infrastructure.

This procedure deploys a Single-Node Single-Drive MinIO server onto Docker.

docker command (using local data volume):

```zsh
% docker run -d \
   -p 9000:9000 \
   -p 9001:9001 \
   --name minio \
   -v minio_data:/data \
   -e "MINIO_ROOT_USER=root" \
   -e "MINIO_ROOT_PASSWORD=P@ssw0rd" \
   quay.io/minio/minio server /data --console-address ":9001"
```

docker command (using data volume):

```zsh
% mkdir -p ~/minio/data

% docker run -d \
   -p 9000:9000 \
   -p 9001:9001 \
   --name minio \
   -v ~/minio/data:/data \
   -e "MINIO_ROOT_USER=ROOTNAME" \
   -e "MINIO_ROOT_PASSWORD=CHANGEME123" \
   quay.io/minio/minio server /data --console-address ":9001"
```


docker compose command:

```yml
# =========================================
# MinIO
#
# Requires:
# - Docker Engine 19.03.0+
# - Docker Compose 1.27.0+
# =========================================

networks:
  # Reverse proxy network
  proxy-net:
    name: proxy-net
    external: true

volumes:
  minio-vol:
    name: minio-vol
    external: true

services:
  minio:
    image: minio/minio:latest
    command: server /data --console-address ':8900'
    environment:
      # - MINIO_ROOT_USER
      # - MINIO_ROOT_PASSWORD
      - MINIO_PROMETHEUS_AUTH_TYPE=public
      # - MINIO_DOMAIN=minio.${DOMAIN}
      # - MINIO_BROSER_REDIRECT_URL=https://adm-minio.${DOMAIN}
      # - MINIO_SERVER_URL=https://minio.${DOMAIN}
    # Labels for Traefik proxy
    # labels:
    #   - traefik.enable=true
    #   - traefik.http.routers.minio.entrypoints=websecure
    #   - traefik.http.routers.minio.rule=Host(`minio.${DOMAIN}`)
    #   - traefik.http.routers.minio.service=minio
    #   - traefik.http.routers.minio.tls=true
    #   - traefik.http.routers.minio.tls.certresolver=le
    #   - traefik.http.routers.minio.tls.domains[0].main=${DOMAIN}
    #   - traefik.http.routers.minio.tls.domains[0].sans=*.${DOMAIN}
    #   - traefik.http.routers.minio.priority=10
    #   - traefik.http.services.minio.loadbalancer.server.port=9000
    #   - traefik.http.services.minio.loadbalancer.server.scheme=http
    #   - traefik.http.services.minio.loadbalancer.passHostHeader=true
    #   - traefik.http.routers.minio-adm.entrypoints=websecure
    #   - traefik.http.routers.minio-adm.rule=Host(`adm-minio.${DOMAIN}`)
    #   - traefik.http.routers.minio-adm.service=minio-adm
    #   - traefik.http.routers.minio-adm.tls=true
    #   - traefik.http.routers.minio-adm.tls.certresolver=le
    #   - traefik.http.routers.minio-adm.tls.domains[0].main=${DOMAIN}
    #   - traefik.http.routers.minio-adm.tls.domains[0].sans=*.${DOMAIN}
    #   - traefik.http.routers.minio-adm.priority=11
    #   - traefik.http.services.minio-adm.loadbalancer.server.port=8900
    #   - traefik.http.services.minio-adm.loadbalancer.server.scheme=http
    #   - traefik.http.services.minio-adm.loadbalancer.passHostHeader=true
    networks:
      - proxy-net
    # ports:
    #   - 9000:9000
    #   - 8900:8900
    restart: unless-stopped
    volumes:
      - minio-vol:/data
    healthcheck:
      test: ['CMD', 'curl', '-f', 'http://localhost:9000/minio/health/live']
      start_period: 20s
      interval: 5s
      retries: 5
      timeout: 4s
```

Access the MinIO Console by navigating to http://127.0.0.1:9001. While port 9000 is used for connecting to the API, MinIO automatically redirects browser access to the MinIO Console (9001).

Log in to the Console with the credentials you defined in the MINIO_ROOT_USER and MINIO_ROOT_PASSWORD environment variables.

# MinIO Client SDK for .NET

MinIO Client SDK provides higher level APIs for MinIO and Amazon S3 compatible cloud storage services.

```zsh
% dotnet add package minio
```

# MinIO Client Example for ASP.NET

```cs
var endpoint = "play.min.io";
var accessKey = "minioadmin";
var secretKey = "minioadmin";
var secure = true;

// Add Minio using the default endpoint
builder.Services.AddMinio(accessKey, secretKey);

// Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
builder.Services.AddMinio(configureClient => configureClient
	.WithEndpoint(endpoint)
	.WithCredentials(accessKey, secretKey)
	.Build());
```

```cs


```




# References

* https://min.io/docs/minio/container/index.html
* https://min.io/docs/minio/linux/developers/dotnet/minio-dotnet.html
* https://min.io/docs/minio/linux/developers/dotnet/API.html


















``` csharp

https://docs.min.io/docs/multi-tenant-minio-deployment-guide.html

                byte[] data = System.Text.Encoding.UTF8.GetBytes("hello world");                


                PutObjectArgs putObjectArgs = new PutObjectArgs();

                putObjectArgs.WithBucket("deneme");

                putObjectArgs.WithContentType("text/plain");

                putObjectArgs.WithStreamData(new MemoryStream(data));
                putObjectArgs.WithObject("emre/hello.txt");
                putObjectArgs.WithObjectSize(11);

            

                await minioc.PutObjectAsync(putObjectArgs);

using Minio;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioApp
{
    public class MinioWrapper
    {
        private MinioClient minioc;

        public MinioWrapper()
        {
            minioc = new MinioClient()
                .WithEndpoint("127.0.0.1:9000")
                .WithCredentials("minioadmin", "minioadmin")
                // .WithSSL()
                .Build();
        }

        public async Task<List<Bucket>> GetBuckects()
        {
            var buckets = await minioc.ListBucketsAsync();
            return buckets.Buckets;
        }

        public async void CreateBucket(string bucketName)
        {
            BucketExistsArgs args = new BucketExistsArgs();
            args.WithBucket(bucketName);

            bool isFound = await minioc.BucketExistsAsync(args);

            if (!isFound)
            {
                MakeBucketArgs mbargs = new MakeBucketArgs();
                mbargs.WithBucket(bucketName);

                await minioc.MakeBucketAsync(mbargs);
            }
        }
        //https://docs.min.io/docs/dotnet-client-api-reference.html
        //https://github.com/minio/minio-dotnet
        //public void PutFile(IFormFile file)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    file.CopyTo(stream);
        //    stream.Position = 0;

        //    string bucketName = "medium";
        //    string objectName = Guid.NewGuid().ToString().Substring(0, 7) + Path.GetExtension(file.FileName);
        //    string contentType = file.ContentType;

        //    await minioClient.PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType);
        //}
    }
}


```



``` csharp

/*

https://docs.min.io/docs/minio-docker-quickstart-guide.html
https://hub.docker.com/r/minio/minio/tags
https://min.io/download#/windows
https://medium.com/emrekizildas/minio-nedir-net-core-projelerinizde-nas%C4%B1l-kullan%C4%B1l%C4%B1r-9504f67f33c8
https://okankaradag.com/asp-net/asp-net-core/asp-net-core-minio-kullanimi
https://docs.min.io/docs/dotnet-client-api-reference.html
https://github.com/minio/minio-dotnet
*/

using Minio;
using Minio.Exceptions;
using MinioApp;

MinioWrapper m = new MinioWrapper();

var buckets = await m.GetBuckects();

if (buckets.Count == 0) 
{
    m.CreateBucket("FirstBucket".ToLower());
    buckets = await m.GetBuckects();
}

buckets.ForEach(b => Console.WriteLine(b.Name));

Console.ReadLine();




/*
 
// 1. public MinioClient(String endpoint)
MinioClient minioClient = new MinioClient("play.min.io");

// 2. public MinioClient(String endpoint, String accessKey, String secretKey)
MinioClient minioClient = new MinioClient("play.min.io",
                                          accessKey:"Q3AM3UQ867SPQQA43P2F",
                                          secretKey:"zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG"
                                          ).WithSSL();

// 3. Initializing minio client with proxy
IWebProxy proxy = new WebProxy("192.168.0.1", 8000);
MinioClient minioClient = new MinioClient("my-ip-address:9000", "minio", "minio123").WithSSL().WithProxy(proxy);

// 4. Initializing minio client with temporary credentials
MinioClient minioClient = new MinioClient("my-ip-address:9000", "tempuserid", "temppasswd", sessionToken:"sessionToken");
 
 */

//namespace FileUploader
//{
//    class FileUpload
//    {
//        static void Main(string[] args)
//        {
//            var endpoint = "127.0.0.1:9000";
//            var accessKey = "Q3AM3UQ867SPQQA43P2F";
//            var secretKey = "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG";
//            try
//            {
//                var minio = new MinioClient(endpoint, accessKey, secretKey); //.WithSSL();
//                FileUpload.Run(minio).Wait();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            Console.ReadLine();
//        }

//        // File uploader task.
//        private async static Task Run(MinioClient minio)
//        {
//            var bucketName = "mymusic";
//            var location = "us-east-1";
//            var objectName = "golden-oldies.zip";
//            var filePath = "C:\\Users\\username\\Downloads\\golden_oldies.mp3";
//            var contentType = "application/zip";

//            try
//            {
//                // Make a bucket on the server, if not already present.
//                bool found = await minio.BucketExistsAsync(bucketName);
//                if (!found)
//                {
//                    await minio.MakeBucketAsync(bucketName, location);
//                }
//                // Upload a file to bucket.
//                await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
//                Console.WriteLine("Successfully uploaded " + objectName);
//            }
//            catch (MinioException e)
//            {
//                Console.WriteLine("File Upload Error: {0}", e.Message);
//            }
//        }
//    }
//}

```