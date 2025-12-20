namespace EcomAPI.Responses
{
    public class ApiResponse
    {
        public bool Success { get; set; } = false;
        public int Status { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}