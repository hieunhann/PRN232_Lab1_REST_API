using System;
using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.Services.Models
{
    public class SemesterBusinessModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public ICollection<CourseBusinessModel> Courses { get; set; } = new List<CourseBusinessModel>();
    }
}
