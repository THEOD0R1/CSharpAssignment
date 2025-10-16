public class ValidatorResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public static ValidatorResponse Success(string? successMessage = null)
        => new() { IsSuccess = true, Message = successMessage };

    public static ValidatorResponse Failed(string? errorMessage = null)
        => new() { IsSuccess = false, Message = errorMessage };
}

public class ValidatorResponse<T> : ValidatorResponse
{
    public T? Content { get; set; }

    public static ValidatorResponse<T> Success(T content, string? successMessage = null)
        => new() { IsSuccess = true, Message = successMessage, Content = content };

    public static new ValidatorResponse<T> Failed(string? errorMessage = null)
        => new() { IsSuccess = false, Message = errorMessage };
}
