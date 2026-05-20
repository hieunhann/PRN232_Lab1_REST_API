namespace PRN232.LAB_1_REST_API.Services.Models.Responses
{
    public class PagedResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
