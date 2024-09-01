using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace src.App_Lib;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            var serverResponse = new ServiceResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusMessage = HttpStatusCode.InternalServerError.ToString(),
                IsSuccess = false,
                ResultCode = ServiceResultCodes.Exception,
                ResultMessage = context.Exception?.Message,
                Exception = context.Exception?.ToStr()
            };

            context.Result = new JsonResult(serverResponse);

            context.ExceptionHandled = true;
        }



    }
}

public static class AppExt
{
    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static string ToJson2(this Exception ex, bool includeInnerException = true, bool includeStackTrace = false, JsonSerializerOptions? options = null)
    {



        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.None

        };

        var res = JsonConvert.SerializeObject(ex, jsonSerializerSettings);

        // ArgumentNullException.ThrowIfNull(ex);

        return res;
    }

    public static Dictionary<string, string> ToDict(this Exception exception)
    {
        var properties = exception.GetType()
            .GetProperties();
        var fields = properties
            .Select(property => new
            {
                Name = property.Name,
                Value = property.GetValue(exception, null)
            })
            .Select(x => $"{x.Name} = {(x.Value != null ? x.Value.ToString() : string.Empty)}")
            .ToDictionary(k => k, v => v);
        return fields;
    }

    public static string ToStr(this Exception exception)
    {
        StringBuilder sb = new StringBuilder();

        Exception? ex = exception;

        int level = 1;

        while (ex != null)
        {
            var st = new StackTrace(ex, true);

            var frame = st.GetFrame(0);



            if (frame != null)
            {
                sb.Append($"Exception in {frame?.GetFileName()} (#{frame?.GetFileLineNumber()})");
            }

            sb.Append($"[{ex.GetType()} {level++}].").Append(ex?.Message).Append(". ");

            ex = ex?.InnerException;

        }


        return sb.ToString();
    }
}