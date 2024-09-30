using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Result;
using src.Models;

namespace src.App_Lib
{
    public class MinioManager(IMinioClient minioClient)
    {
        public MinioInstanceModel GetInstance() => new MinioInstanceModel
        {
            Endpoint = minioClient.Config.Endpoint,
            AccessKey = minioClient.Config.AccessKey
        };        

        public async Task<List<Bucket>?> ListBuckets()
        {
            ListAllMyBucketsResult buckets = await minioClient.ListBucketsAsync();
            var result = buckets.Buckets.OrderBy(b => b.Name).ToList();
            return result;
        }

        public async Task AddBucket(string BucketName)
        {
            if (string.IsNullOrEmpty(BucketName) || BucketName.Length <= 3)
            {
                throw new Exception("Bucket name must be greater than 3 char long.");
            }

            BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

            if (!await BucketExists(BucketName))
            {
                MakeBucketArgs makeArgs = new MakeBucketArgs()
                    .WithBucket(BucketName);

                await minioClient.MakeBucketAsync(makeArgs);
            }
            else
            {
                throw new Exception("This bucket name is in use.");
            }
        }

        public async Task RemoveBucket(string BucketName)
        {
            if (string.IsNullOrEmpty(BucketName) || BucketName.Length <= 3)
            {
                throw new Exception("Bucket name must be greater than 3 char long.");
            }

            BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

            if (await BucketExists(BucketName))
            {
                if(!await IsBucketEmpty(BucketName))
                {
                    throw new Exception("This bucket is NOT empty.");
                }

                RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs()
                    .WithBucket(BucketName);

                await minioClient.RemoveBucketAsync(removeBucketArgs);
            }
            else
            {
                throw new Exception("This bucket name is NOT exist.");
            }
        }

        public async Task<bool> BucketExists(string bucketName)
        {
            BucketExistsArgs bucketArgs = new BucketExistsArgs()
                .WithBucket(bucketName);

            bool isFound = await minioClient.BucketExistsAsync(bucketArgs);

            return isFound;
        }

        public async Task<bool> IsBucketEmpty(string bucketName)
        {
            try
            {
                var listObjectsArgs = new ListObjectsArgs()
                    .WithBucket(bucketName)
                    .WithRecursive(false)
                ; 

                var isEmpty = true;

                await foreach (var item in minioClient.ListObjectsEnumAsync(listObjectsArgs))
                {
                    isEmpty = false;
                    break;
                }

                return isEmpty;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<List<Item>> GetFiles(string bucketName)
        {
            ListObjectsArgs statObjectArgs = new ListObjectsArgs()
                .WithBucket(bucketName);

            var list = await minioClient.ListObjectsEnumAsync(statObjectArgs)
                .ToListAsync();

            return list;
        }

        public async Task<ObjectStat> GetFileMeta(string bucketName, string fileName)
        {
            StatObjectArgs args = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
            ;

            var stat = await minioClient.StatObjectAsync(args);

            return stat;
        }

        public async Task<string> UploadFile(string BucketName, IFormFile file)
        {
            if (string.IsNullOrEmpty(BucketName) || BucketName.Length <= 3)
            {
                throw new Exception("Bucket name must be greater than 3 char long.");
            }

            BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

            if (!await BucketExists(BucketName))
            {
                throw new Exception("Bucket NOT found!");
            }

            try
            {
                string extension = Path.GetExtension(file.FileName);

                string newName = $"{Guid.NewGuid()}{extension}";

                PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(BucketName)
                    .WithContentType(file.ContentType)
                    .WithStreamData(file.OpenReadStream())
                    .WithObject(newName)
                    .WithObjectSize(file.Length)
                ;

                await minioClient.PutObjectAsync(putObjectArgs);

                return newName;
            }
            catch
            {
                throw;
            }
        }
    
        public async Task<(Stream fileStream, ObjectStat minioObjStat)> DownloadFile(string bucketName, string fileName)
        {
            try
            {
                var memoryStream = new MemoryStream();

                GetObjectArgs getObjectArgs2 = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream))
                ;

                var oStat = await minioClient.GetObjectAsync(getObjectArgs2);

                memoryStream.Position = 0;
                
                return (memoryStream, oStat);
            }
            catch
            {
                throw;
            }
        }
    }
}

/*
	static async IAsyncEnumerable<int> GetNumbersAsync()
	{
		for (int i = 0; i < 5; i++)
		{
			await Task.Delay(1000); // Simulate an asynchronous operation
			yield return i;
		}
	}
*/