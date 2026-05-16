namespace Ecommerce525.API.DTOs.Response
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data  { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
