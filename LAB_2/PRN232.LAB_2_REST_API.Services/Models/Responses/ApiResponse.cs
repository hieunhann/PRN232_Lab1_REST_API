using System.Text.Json.Serialization;

namespace PRN232.LAB_2_REST_API.Services.Models.Responses
{
    public class ApiResponse<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PagedResponse? Pagination { get; set; }
        
        public bool Success { get; set; }
        
        public string Message { get; set; } = string.Empty;
        
        public T? Data { get; set; }
        
        public object? Errors { get; set; }
    }
}
