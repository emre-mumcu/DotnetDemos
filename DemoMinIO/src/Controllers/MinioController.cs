using Microsoft.AspNetCore.Mvc;
using src.App_Lib;

namespace src.Controllers;

public class MinioController(MinioManager minioManager) : Controller
{
    /*
        private readonly MinioManager minioManager;
    private readonly IMinioClient _minioClient;

	public HomeController(IMinioClient minioClient)
	{
        if(minioClient is null)
        {
            var minioConfig = HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Minio").Get<MinioConfig>();
            minioClient = new MinioClient()
                .WithEndpoint(minioConfig!.Endpoint)                
                .WithCredentials(minioConfig!.AccessKey, minioConfig.SecretKey)
                .WithSSL()
                .Build();            
        }

        _minioClient = minioClient;

        minioManager = new MinioManager(minioClient);
	}
    */

    [HttpGet]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Index()
    {
        string? Message = TempData["Message"]?.ToString() ?? null;

        var model = (await minioManager.ListBuckets(), minioManager.GetInstance(), Message);        

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddBucket(string BucketName)
    {
        try
        {
            await minioManager.AddBucket(BucketName);
            TempData["Message"] = "Bucket added";
        }
        catch (System.Exception ex)
        {
            TempData["Message"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ContentResult> RemoveBucket(string BucketName)
    {
        string message = string.Empty;

        try
        {
            await minioManager.RemoveBucket(BucketName);
            message = "Bucket removed";
        }
        catch (System.Exception ex)
        {
            message = ex.Message;
        }

        return Content(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<string> UploadFile(string FileBucketName, IFormFile file)
    {
        string message = string.Empty;

        try
        {
            string key = await minioManager.UploadFile(FileBucketName, file);
            message = $"File uploaded with key {key}";
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }

        return message;
    }

    [HttpGet]
    public async Task<FileResult> DownloadFile(string bucketName, string fileName)
    {
        try
        {
            var result = await minioManager.DownloadFile(bucketName, fileName);

            //"application/octet-stream"            
            FileStreamResult fileStreamResult = File(result.fileStream, result.minioObjStat.ContentType, result.minioObjStat.ObjectName);

            return fileStreamResult;
        }
        catch (System.Exception ex)
        {
            string exx = ex.Message;
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles(string BucketName)
    {
        try
        {
            var result = await minioManager.GetFiles(BucketName);            
            // var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            // return json;
            return View(viewName: "Files", model: (result, BucketName));
        }
        catch
        {
            throw;
        }
    }

    [HttpGet]
    public async Task<string> GetFileMeta(string BucketName, string FileName)
    {
        try
        {
            var result = await minioManager.GetFileMeta(BucketName, FileName);
            var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            return json;
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<string> GetFileMeta2(string BucketName, string FileName)
    {
        try
        {
            var result = await minioManager.GetFileMeta(BucketName, FileName);
            var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            return json;
        }
        catch
        {
            throw;
        }
    }


}

// https://localhost:5001/Home/GetFiles?BucketName=mybucket
// https://localhost:5001/Home/GetFileMeta?BucketName=mybucket&FileName=f850a608-2aef-49ba-922f-623df53acb31.txt