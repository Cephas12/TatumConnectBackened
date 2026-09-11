namespace TatumConnectBackened.Responses
{
    public class ApiBaseResponse
    {
    }
    public class ApiError
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ApiError() { }

        public ApiError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
    public class  ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ApiError>? Errors { get; set; }
        public object? Meta { get; set; }

        public ApiResponse()
        { 
        }    
        public ApiResponse (
            bool success,
            string message,
            T? data = default,
            List<ApiError>? errors = null,
            object? meta = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            Meta = meta;
        }
        public static ApiResponse<T> Ok(T data, string message = "Request successful")
        {
            return new ApiResponse<T> {
                Success = true,
                Message = message,
                Data = data
            };
        }
        public static ApiResponse<T> Fail(
            string message,
            List<ApiError>? errors = null)
        {
            return new ApiResponse<T> {
                Success = false,
                Message = message,
                Errors = errors,
            };
        }
    }
}
