namespace Infrastructure.Models;

public class ResponseResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Error { get; set; }

    public static ResponseResult Ok()
    {
        return new ResponseResult
        {
            Success = true,
            StatusCode = 200,
        };
    }

    public static ResponseResult Fail(int statusCode, string? message = null)
    {
        return new ResponseResult
        {
            Success = false,
            StatusCode = statusCode,
            Error = message
        };
    } 
}
public class ResponseResult<T> : ResponseResult
{
    public T? Content { get; set; }

    public static ResponseResult<T> Ok(T content)
    {
        return new ResponseResult<T>
        {
            Success = true,
            StatusCode = 200,
            Content = content
        };
    }

    public static new ResponseResult<T> Fail(int statusCode, string? message = null)
    {
        return new ResponseResult<T>
        {
            Success = false,
            StatusCode = statusCode,
            Error = message
        };
    }
}
