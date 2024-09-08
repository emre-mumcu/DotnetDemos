using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;
using Minio.Exceptions;
using src.App_Lib;

namespace src.Controllers;

public class EskiController : Controller
{
	private readonly IMinioClient _minioClient;

	public EskiController(IMinioClient minioClient)
    {
		_minioClient = minioClient;
	}

	[HttpGet]
	[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
	public async Task<IActionResult> Index()
    {
		// var result = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("bucketID")).ConfigureAwait(false);

		var buckets = _minioClient.ListBucketsAsync().ConfigureAwait(false);

		return View();
    }



	[HttpPost]
	public async Task<IActionResult> UploadFile(IFormFile file)
	{
MinioService minioService = new MinioService(_minioClient as MinioClient, "testbucket");

		try
		{
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
			return RedirectToAction("Index");





		}
		catch (MinioException e)
		{
			Console.WriteLine("Error occurred: " + e);
		}

		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> DownloadFile(string fileName)
	{
		MinioService minioService = new MinioService(_minioClient as MinioClient, "testbucket");
		
		var filePath = Path.GetTempFileName();

		await minioService.DownloadFileAsync(fileName);

		// using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
		// {
		// 	await minioService.DownloadFileAsync(fileName, fileStream);
		// }



		var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
		System.IO.File.Delete(filePath);

		return File(fileBytes, "application/octet-stream", fileName);
	}
}

// l3xkMpqB5aWWH0IYSgpx
// sJ0CTR3WfKwOam3CHrGE0dgX2P1jK22sYkPVU5gp

// UStBVq28XkzEiNkBLcdL
// JFih0tREGrz1rdOSMj3PLIroVMD2wG5AN3rdL6O9