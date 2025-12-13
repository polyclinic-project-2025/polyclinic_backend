namespace PolyclinicApplication.Common.Results
{
    public class ApiResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ApiResponse(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static ApiResponse Success(string message = "Operación exitosa")
            => new("SUCCESS", message);

        public static ApiResponse Error(string message, string code = "ERROR")
            => new(code, message);

        public static ApiResponse NotFound(string message = "Recurso no encontrado")
            => new("NOT_FOUND", message);

        public static ApiResponse BadRequest(string message)
            => new("BAD_REQUEST", message);

        public static ApiResponse ValidationError(string message)
            => new("VALIDATION_ERROR", message);
    }
}