```cs
Aes aesEncryption = Aes.Create();
aesEncryption.KeySize = 256;
aesEncryption.GenerateKey();			
var ssec = new SSEC(aesEncryption.Key);
var progress = new Progress<ProgressReport>(progressReport =>
{
	// Progress events are delivered asynchronously (see remark below)
	Console.WriteLine($"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes");

	// if (progressReport.Percentage != 100)
	// 	Console.SetCursorPosition(0, Console.CursorTop - 1);
	// else Console.WriteLine();
});

if (file != null && file.Length > 0)
{
	using (var stream = file.OpenReadStream())
	{
		await minioService.UploadFileAsync(file.FileName, stream, file.ContentType, ssec, progress);
	}
}



var putObjectArgs = new PutObjectArgs()
	.WithBucket(_bucketName)
	.WithObject(objectName)
	.WithFileName(file.FileName)
	.WithStreamData(fileStream)
	.WithObjectSize(fileStream.Length)
	.WithContentType(contentType)
	.WithServerSideEncryption(ssec)
	.WithProgress(progress)
	;	

var args = new GetObjectArgs()
	.WithBucket(_bucketName)
	.WithObject(objectName)
	// .WithCallbackStream(x =>
	// {
	// 	x.CopyTo(downloadStream);
	// 	downloadStream.Seek(0, SeekOrigin.Begin);
	// });
	.WithOffsetAndLength(1024L, 4096L)
	.WithCallbackStream(async (stream, cancellationToken) =>
	{
		var fileStream = File.Create(objectName);
		await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
		await fileStream.DisposeAsync().ConfigureAwait(false);
		var writtenInfo = new FileInfo(objectName);
		var file_read_size = writtenInfo.Length;
		// Uncomment to print the file on output console
		// stream.CopyTo(Console.OpenStandardOutput());
		Console.WriteLine(
			$"Successfully downloaded object with requested offset and length {writtenInfo.Length} into file");
		stream.Dispose();
	});		
```

``` csharp

https://docs.min.io/docs/multi-tenant-minio-deployment-guide.html

byte[] data = System.Text.Encoding.UTF8.GetBytes("hello world"); 
PutObjectArgs putObjectArgs = new PutObjectArgs();
putObjectArgs.WithBucket("deneme");
putObjectArgs.WithContentType("text/plain");
putObjectArgs.WithStreamData(new MemoryStream(data));
putObjectArgs.WithObject("emre/hello.txt");
putObjectArgs.WithObjectSize(11);
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

// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-6.0#attribute-routing-for-rest-apis
// https://docs.min.io/docs/dotnet-client-quickstart-guide.html
// https://docs.min.io/docs/dotnet-client-api-reference.html


docker exec -it <mycontainer> sh
docker exec -it minio sh

cd /opt
mkdir certs

docker ps

docker cp [SOURCE_PATH] [CONTAINER_ID]:[DESTINATION_PATH]

To enable TLS for MinIO. You may use TLS certificates from a well-known Certificate Authority, an internal or private CA, or self-signed certs.

mkcert -cert-file minio.crt -key-file minio.key localhost 127.0.0.1

Start the MinIO container with the `minio/minio:latest server --certs-dir` parameter and specify the path to a directory in which MinIO searches for certificates. You must mount a local host volume to that path when starting the container to ensure the MinIO Server can access the necessary certificates.

Place the TLS certificates for the default domain (e.g. minio.example.net) in the specified directory, with the private key as private.key and public certificate as public.crt. For example:

/opts/certs
  private.key
  public.crt

Move the certificates to the local host machine path that the container mounts to its --certs-dir path. When the MinIO container starts, the server searches the specified location for certificates and uses them to enable TLS. Applications can use the public.crt as a trusted Certificate Authority to allow connections to the MinIO deployment without disabling certificate validation.

If you are reconfiguring an existing deployment that did not previously have TLS enabled, update MINIO_VOLUMES to specify https instead of http. You may also need to update URLs used by applications or clients.  

https://min.io/docs/minio/container/operations/network-encryption.html




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