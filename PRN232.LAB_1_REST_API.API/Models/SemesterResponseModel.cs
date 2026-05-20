using System;
using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.API.Models
{
    public class SemesterResponseModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public ICollection<CourseResponseModel> Courses { get; set; } = new List<CourseResponseModel>();
    }
}
