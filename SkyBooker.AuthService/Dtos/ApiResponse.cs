namespace SkyBooker.AuthService.Dtos;

// Standard response format used by ALL our APIs.
// Every API response looks like: { "success": true/false, "data": {...}, "message": "..." }

// Version WITH data (use when returning data like user profile, token, etc.)
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;

    // Use this when the operation was successful
    public static ApiResponse<T> Ok(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    // Use this when the operation failed
    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message
        };
    }
}

// Version WITHOUT data (use for simple messages like "Logged out successfully")
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Ok(string message = "Operation successful")
    {
        return new ApiResponse { Success = true, Message = message };
    }

    public static ApiResponse Fail(string message)
    {
        return new ApiResponse { Success = false, Message = message };
    }
}
