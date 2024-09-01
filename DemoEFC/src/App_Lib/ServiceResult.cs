using System.Net;
using Newtonsoft.Json;

namespace src.App_Lib;

public class ServiceResult<T> : ServiceResult
{    
    [JsonProperty(Order = 60)] public new T? ResultData { get; set; }
}

public class ServiceResult
{
    [JsonProperty(Order = 10)] public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    [JsonProperty(Order = 20)] public string StatusMessage { get; set; } = HttpStatusCode.OK.ToString();
    [JsonProperty(Order = 30)] public bool IsSuccess { get; set; } = true;
    [JsonProperty(Order = 40)] public ServiceResultCodes ResultCode { get; set; } = ServiceResultCodes.Success;
    [JsonProperty(Order = 50)] public string? ResultMessage { get; set; } = ServiceResultCodes.Success.ToString();
    [JsonProperty(Order = 60)] public string? ResultData { get; } = null;
    [JsonProperty(Order = 70)] public string? Exception { get; set; } = null;
}

public enum ServiceResultCodes
{
    Unset = -1,
    Fail = 0,
    Success = 1,
    InvalidRequest = 2,
    Unauthenticated = 3,
    Unauthorized = 4,
    OperationNOTAllowed = 5,
    StatusCodeException = 6,
    Error = 9,
    Exception = 10,
}


public class ServiceResults
{
    public static ServiceResult<T> Success<T>(T? data, string? message = null)
    {
        return new ServiceResult<T>() {
            StatusCode = HttpStatusCode.OK,
            StatusMessage = HttpStatusCode.OK.ToString(),
            IsSuccess = true,
            ResultCode = ServiceResultCodes.Success,
            ResultMessage = message ?? "Success",
            ResultData = data
        }; 
    }

    public static ServiceResult Fail(string? message)
    {
        return new ServiceResult()
        {
            StatusCode = HttpStatusCode.OK,
            StatusMessage = HttpStatusCode.OK.ToString(),
            IsSuccess = false,
            ResultCode = ServiceResultCodes.Fail,
            ResultMessage = message ?? "Fail"        
        };
    }

    public static ServiceResult Exception(Exception? ex)
    {
        return new ServiceResult()
        {
            StatusCode = HttpStatusCode.OK,
            StatusMessage = HttpStatusCode.OK.ToString(),
            IsSuccess = false,
            ResultCode = ServiceResultCodes.Exception,
            ResultMessage = ex?.Message ?? "Exception",
            Exception = ex?.ToStr()
        };
    }
}