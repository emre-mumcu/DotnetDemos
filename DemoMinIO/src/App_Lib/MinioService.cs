using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;

namespace src.App_Lib
{
	public class MinioService
    {
        private readonly MinioClient _minioClient;
        private readonly string _bucketName;
        public MinioService(MinioClient minioClient, string bucketName = "default")
		{
            _bucketName = bucketName;
            _minioClient = minioClient;			
		}

		public async Task<bool> BucketExistsAsync()
		{
			var args = new BucketExistsArgs().WithBucket(_bucketName);
			return await _minioClient.BucketExistsAsync(args);
		}

		public async Task UploadFileAsync(string objectName, Stream fileStream, string contentType, SSEC ssec, IProgress<ProgressReport> progress)
		{
			var putObjectArgs = new PutObjectArgs()
				.WithBucket(_bucketName)
				.WithObject(objectName)
				// .WithFileName(file.FileName)
				.WithStreamData(fileStream)
				.WithObjectSize(fileStream.Length)
				.WithContentType(contentType)
				//.WithServerSideEncryption(ssec)
				//.WithProgress(progress)
				;

			await _minioClient.PutObjectAsync(putObjectArgs);
		}

		public async Task DownloadFileAsync(string objectName)
		{
			var downloadStream = new MemoryStream();
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
			_ = await _minioClient.GetObjectAsync(args).ConfigureAwait(false);

			//var getObjectArgs = new GetObjectArgs()
			//.WithBucket(_bucketName)
			//.WithObject(objectName)
			//.WithFile(filePath)
			;

			//await _minioClient.GetObjectAsync(getObjectArgs);
			//await _minioClient.GetObjectAsync(getObjectArgs, filePath);
			//await _minioClient.GetObjectAsync(getObjectArgs, destinationStream);

			/*
			var memStream = new MemoryStream();  GetObjectArgs getObjectArgs = new GetObjectArgs()                                     .WithBucket(myBucket)                                     .WithObject(myObject)                                     .WithCallbackStream((stream) =>                                     {                                         stream.CopyTo(memStream);                                     });  object objectData = await minio.GetObjectAsync(getObjectArgs);  return memStream;
			*/
		}

	}
}