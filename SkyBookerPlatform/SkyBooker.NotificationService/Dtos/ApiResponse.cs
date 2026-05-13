namespace SkyBooker.NotificationService.Dtos;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse<T> Ok(T data, string message = "Operation successful")
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Data = default, Message = message };
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Ok(string message = "Operation successful")
        => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message)
        => new() { Success = false, Message = message };
}
