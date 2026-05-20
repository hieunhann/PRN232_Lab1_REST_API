using System.Text.Json.Serialization;

namespace PRN232.LAB_1_REST_API.API.Models
{
    public class ApiResponse<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationMetadata? Pagination { get; set; }
        
        public bool Success { get; set; }
        
        public string Message { get; set; } = string.Empty;
        
        public T? Data { get; set; }
        
        public object? Errors { get; set; }
    }
}
