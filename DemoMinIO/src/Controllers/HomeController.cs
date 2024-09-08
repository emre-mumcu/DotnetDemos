using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace src.Controllers
{
	public class HomeController : Controller
	{
		public async Task<IActionResult> Index()
		{
			var minioClient = new MinioClient()
				.WithEndpoint("127.0.0.1:9000")
				.WithCredentials("root", "P@ssw0rd")
				// .WithSSL()
				.Build();

			var buckets = await minioClient.ListBucketsAsync();

			buckets.Buckets.OrderBy(b => b.Name);

			return View(model: buckets.Buckets.ToList());
		}

		[HttpGet]
		public IActionResult AddBucket()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddBucket(string BucketName)
		{
			var minioClient = new MinioClient()
	.WithEndpoint("127.0.0.1:9000")
	.WithCredentials("root", "P@ssw0rd")
	// .WithSSL()
	.Build();

			BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

			BucketExistsArgs args = new BucketExistsArgs();
			args.WithBucket(BucketName);

			bool isFound = await minioClient.BucketExistsAsync(args);

			if (!isFound)
			{
				MakeBucketArgs mbargs = new MakeBucketArgs();
				mbargs.WithBucket(BucketName);

				await minioClient.MakeBucketAsync(mbargs);
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> RemoveBucket(string BucketName)
		{
            var minioClient = new MinioClient()
.WithEndpoint("127.0.0.1:9000")
.WithCredentials("root", "P@ssw0rd")
// .WithSSL()
.Build();
            BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

			BucketExistsArgs args = new BucketExistsArgs();
			args.WithBucket(BucketName);

			bool isFound = await minioClient.BucketExistsAsync(args);

			if (isFound)
			{
				RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs();
				removeBucketArgs.WithBucket(BucketName);

				await minioClient.RemoveBucketAsync(removeBucketArgs);

				//Console.Log( $"Bucket **{BucketName}** removed.");
			}
			//else
                //Console.Log($"Bucket **{BucketName}** NOT found.");

                return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<string> PutObject(string FileBucketName, IFormFile file)
		{
			var minioClient = new MinioClient()
			.WithEndpoint("127.0.0.1:9000")
			.WithCredentials("root", "P@ssw0rd")
			// .WithSSL()
			.Build();

			try
			{
				PutObjectArgs putObjectArgs = new PutObjectArgs();

				putObjectArgs.WithBucket(FileBucketName);
				putObjectArgs.WithContentType(file.ContentType);
				putObjectArgs.WithStreamData(file.OpenReadStream());
				putObjectArgs.WithObject(file.FileName);
				putObjectArgs.WithObjectSize(file.Length);

				await minioClient.PutObjectAsync(putObjectArgs);

				return "OK";
			}
			catch (Exception ex)
			{
				return ex.Message;
			}


		}

		[HttpGet]
		public async Task<IActionResult> GetFile(string bucketName, string fileName)
		{
            			var minioClient = new MinioClient()
			.WithEndpoint("127.0.0.1:9000")
			.WithCredentials("root", "P@ssw0rd")
			// .WithSSL()
			.Build();

            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs();

                statObjectArgs.WithBucket(bucketName);
                statObjectArgs.WithObject(fileName);
                ObjectStat ostat = await minioClient.StatObjectAsync(statObjectArgs);

                FileStream outputFileStream = new FileStream("file.txt", FileMode.Create);


                GetObjectArgs getObjectArgs = new GetObjectArgs();
                getObjectArgs.WithBucket(bucketName);
                getObjectArgs.WithObject(fileName);
                getObjectArgs.WithFile(fileName);
                getObjectArgs.WithCallbackStream(stream => stream.CopyTo(outputFileStream));

                ObjectStat obj = await minioClient.GetObjectAsync(getObjectArgs);



            }
            catch 
            {

            }

            return View();
		}

		// https://localhost:7063/Home/GetMeta?BucketName=emre
		[HttpGet]
		public async Task<IActionResult> GetMeta(string BucketName)
		{
			var minioClient = new MinioClient()
.WithEndpoint("127.0.0.1:9000")
.WithCredentials("root", "P@ssw0rd")
// .WithSSL()
.Build();


			ListObjectsArgs statObjectArgs = new ListObjectsArgs();

			statObjectArgs.WithBucket(BucketName);
           

            var list =  minioClient.ListObjectsEnumAsync(statObjectArgs);

            await foreach (var item in list)
            {
                
            }

            var list2 = await list.ToListAsync();

            return View(viewName: "Meta", model: list2);

		}

		// https://localhost:7063/Home/GetMeta2?BucketName=emre&FileName=yoga-8165759_1280.webp
		[HttpGet]
		public async Task<IActionResult> GetMeta2(string BucketName, string FileName)
		{
			var minioClient = new MinioClient()
.WithEndpoint("127.0.0.1:9000")
.WithCredentials("root", "P@ssw0rd")
// .WithSSL()
.Build();


			StatObjectArgs statObjectArgs = new StatObjectArgs();

			statObjectArgs.WithBucket(BucketName);
			statObjectArgs.WithObject(FileName);


			ObjectStat ostat = await minioClient.StatObjectAsync(statObjectArgs);

			

			return View(viewName: "Meta2", model: ostat);


			


			return View("Index");

            // await foreach (var item in GetNumbersAsync())
            // {
            //     Console.WriteLine(item);
            // }

        }

        static async IAsyncEnumerable<int> GetNumbersAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000); // Simulate an asynchronous operation
                yield return i;
            }
        }





    }

    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            var list = new List<T>();
            await foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}


/*
 * 
 * DOCKER

https://docs.docker.com/engine/reference/run/
https://docs.min.io/docs/distributed-minio-quickstart-guide.html

docker run -d \
  -p 9000:9000 \
  -p 9001:9001 \
  --name minio1 \
  -v C:\minio\data:/data \  
  -e "MINIO_ROOT_USER=minioadmin" \
  -e "MINIO_ROOT_PASSWORD=minioadmin" \
  minio/minio server /data --console-address ":9001"

 * Single Mount
   docker run -d -p 9000:9000 -p 9001:9001 --name minio1 -v C:\minio\data:/data -e "MINIO_ROOT_USER=minioadmin" -e "MINIO_ROOT_PASSWORD=minioadmin" minio/minio server /data --console-address ":9001" 

 * COMMAND LINE:
C:\WINDOWS\system32>setx MINIO_ROOT_USER minioadmin
SUCCESS: Specified value was saved.
C:\WINDOWS\system32>setx MINIO_ROOT_PASSWORD minioadmin
SUCCESS: Specified value was saved.
C:\WINDOWS\system32>C:\Minio\minio.exe server C:\Minio\Data --console-address ":9001"

Login:
http://127.0.0.1:9001/login 
**/



/*
** docker-compose.yml
*********************
version: '2'
services:      
  minio1:
    image: minio/minio
    ports: 
      - "9000:9000"
      - "9001:9001"
    volumes:
      - C:\minio\data1:/data1      
      - C:\minio\data2:/data2
    environment:
      - MINIO_ROOT_USER=minioadmin
      - MINIO_ROOT_PASSWORD=minioadmin
      - MINIO_DISTRIBUTED_MODE_ENABLED=yes
      - MINIO_DISTRIBUTED_NODES=minio1,minio2      
      - MINIO_DISTRIBUTED_NODES=minio{1...2}/data{1...2}
    command: 
      server /data1 --console-address ":9001"   
  minio2:
    image: minio/minio
    ports: 
      - "9002:9000"
      - "9003:9001"
    volumes:
      - C:\minio\data1:/data1      
      - C:\minio\data2:/data2
    environment:
      - MINIO_ROOT_USER=minioadmin
      - MINIO_ROOT_PASSWORD=minioadmin
      - MINIO_DISTRIBUTED_MODE_ENABLED=yes
      - MINIO_DISTRIBUTED_NODES=minio1,minio2      
      - MINIO_DISTRIBUTED_NODES=minio{1...2}/data{1...2}
    command: 
      server /data1 --console-address ":9003"  
**/

/*
docker compose up   (in the same folder with docker-compose.yml)
docker-compose down : Containerları sil.
docker-compose down –volumes : Containerları volumeleri ile birlikte kaldır. Bu aşamadan sonra containerın datalarına ulaşamayacağız.
 Bazı durumlarda compose dosyası içerisinde tüm servislerin çalışmasını istemeyebiliriz. Bunun için partial start kavramını kullanacağız. Komutumuz şöyle olacak;

docker-compose up service_name other_service_name;
 */


/*

docker-compose.yml
******************

version: '2'
services:
  minio1:
    image: minio/minio
    ports:
      - "9001:9000"
    environment:
      MINIO_ACCESS_KEY: minio
      MINIO_SECRET_KEY: minio123
    command: server http://minio1/myexport http://minio2/myexport http://minio3/myexport http://minio4/myexport 
  minio2:
    image: minio/minio
    ports:
      - "9002:9000"
    environment:
      MINIO_ACCESS_KEY: minio
      MINIO_SECRET_KEY: minio123
    command: server http://minio1/myexport http://minio2/myexport http://minio3/myexport http://minio4/myexport 
  minio3:
    image: minio/minio
    ports:
      - "9003:9000"
    environment:
      MINIO_ACCESS_KEY: minio
      MINIO_SECRET_KEY: minio123
    command: server http://minio1/myexport http://minio2/myexport http://minio3/myexport http://minio4/myexport 
  minio4:
    image: minio/minio
    ports:
      - "9004:9000"
    environment:
      MINIO_ACCESS_KEY: minio
      MINIO_SECRET_KEY: minio123
    command: server http://minio1/myexport http://minio2/myexport http://minio3/myexport http://minio4/myexport


**
*
*version: '2' #(1)

#(2)
services:
db:
image: mysql:latest
volumes:
- db_data:/var/lib/mysql
restart: always
environment:
MYSQL_ROOT_PASSWORD: 1234
MYSQL_DATABASE: challenge
MYSQL_USER: root
MYSQL_PASSWORD: 1234
ports:
- "3306:3306"
app:
image: aakkus/iyzico-challenge
ports:
- "8080:8080"
depends_on:
- db
- redis

redis:
image: redis:latest
ports:
- "6379:6379"
volumes:
db_data:
 
 */