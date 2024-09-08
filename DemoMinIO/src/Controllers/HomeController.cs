using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace src.Controllers;

public class HomeController : Controller
{
	private readonly IMinioClient _minioClient;

	public HomeController(IMinioClient minioClient)
	{
		// _minioClient = new MinioClient().WithEndpoint("127.0.0.1:9000").WithCredentials("root", "P@aA123456").WithSSL().Build();
		_minioClient = minioClient;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
	public async Task<IActionResult> Index()
	{
		var buckets = await _minioClient.ListBucketsAsync();
		return View(model: buckets.Buckets.OrderBy(b => b.Name).ToList());
	}

	[HttpGet]
	public IActionResult AddBucket() => View();

	[HttpPost]
	public async Task<IActionResult> AddBucket(string BucketName)
	{
		BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

		if (!await BucketExists(BucketName))
		{
			MakeBucketArgs makeArgs = new MakeBucketArgs()
				.WithBucket(BucketName);

			await _minioClient.MakeBucketAsync(makeArgs);
		}

		return RedirectToAction("Index");
	}

	[HttpPost]
	public async Task<IActionResult> RemoveBucket(string BucketName)
	{
		BucketName = BucketName.ToLower(new System.Globalization.CultureInfo("tr-TR"));

		if (await BucketExists(BucketName))
		{
			RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs()
				.WithBucket(BucketName);

			await _minioClient.RemoveBucketAsync(removeBucketArgs);
		}

		return RedirectToAction("Index");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<string> PutObject(string FileBucketName, IFormFile file)
	{
		FileBucketName = FileBucketName.ToLower(new System.Globalization.CultureInfo("tr-TR")); 

		if (!await BucketExists(FileBucketName))
		{
			throw new Exception("Bucket NOT found!");
		}

		try
		{
            string extension = Path.GetExtension(file.FileName);
            string newName = $"{Guid.NewGuid()}.{extension}";

            PutObjectArgs putObjectArgs = new PutObjectArgs()
			    .WithBucket(FileBucketName)
			    .WithContentType(file.ContentType)
			    .WithStreamData(file.OpenReadStream())
			    .WithObject(newName)
			    .WithObjectSize(file.Length)
            ;

			await _minioClient.PutObjectAsync(putObjectArgs);

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


		var list = minioClient.ListObjectsEnumAsync(statObjectArgs);

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

	public async Task<bool> BucketExists(string bucketName)
	{
		BucketExistsArgs bucketArgs = new BucketExistsArgs()
			.WithBucket(bucketName);

		bool isFound = await _minioClient.BucketExistsAsync(bucketArgs);

		return isFound;
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
