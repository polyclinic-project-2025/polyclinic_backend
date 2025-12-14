namespace PolyclinicApplication.Common.Results
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }

        private ApiResult(bool success, T? data, string? message, string? errorCode = null)
        {
            Success = success;
            Data = data;
            Message = message;
            ErrorCode = errorCode;
        }

        public static ApiResult<T> Ok(T data, string message = "Operación exitosa")
            => new(true, data, message);

        public static ApiResult<T> Error(string message, string? errorCode = null)
            => new(false, default, message, errorCode);

        public static ApiResult<T> NotFound(string message = "Recurso no encontrado")
            => new(false, default, message, "NOT_FOUND");

        public static ApiResult<T> BadRequest(string message)
            => new(false, default, message, "BAD_REQUEST");
    }
}