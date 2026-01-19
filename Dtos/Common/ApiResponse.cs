namespace QuanLyTaiSan.Dtos.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }
        //public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        //public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    }
}
