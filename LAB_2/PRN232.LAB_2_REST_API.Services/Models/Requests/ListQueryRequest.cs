using System.ComponentModel;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    /// <summary>
    /// Common query parameters for list querying.
    /// </summary>
    public class ListQueryRequest
    {
        /// <summary>
        /// Search keyword matching the string fields of the entity.
        /// </summary>
        [Description("Search keyword matching the string fields of the entity, e.g., name, email, course code.")]
        public string? Search { get; set; }

        /// <summary>
        /// Sort by property name. Supports multiple fields separated by commas.
        /// </summary>
        [Description("Sort by property name, e.g., fullName or -fullName for descending. Multiple fields can be separated by commas.")]
        public string? Sort { get; set; }

        /// <summary>
        /// Current page number, starting from 1.
        /// </summary>
        [Description("Current page, starting from 1.")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of records per page.
        /// </summary>
        [Description("Number of records per page. Use positive numbers like 10, 20, 50.")]
        public int Size { get; set; } = 10;

        /// <summary>
        /// Selected fields to return in the response.
        /// </summary>
        [Description("List of fields to return, separated by commas. E.g., studentId,fullName,email")]
        public string? Fields { get; set; }

        /// <summary>
        /// Eager load related navigation properties.
        /// </summary>
        [Description("Eager load related navigation properties, separated by commas. E.g., enrollments.course or course.enrollments.student")]
        public string? Expand { get; set; }

        /// <summary>
        /// Dynamic filter expression for the list.
        /// </summary>
        [Description("Dynamic filter expression. E.g., CourseId == 15, Status == \"Active\". Can combine conditions with && or ||.")]
        public string? Filter { get; set; }
    }
}
