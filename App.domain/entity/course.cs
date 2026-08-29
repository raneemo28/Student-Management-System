using System;
using System.Collections.Generic;
using System.Text;

namespace App.domain.entity
{
    public class Course
    {
        public string? Course_id { get; set; }
        public string? Course_name { get; set; }
        public string? Instructor_id { get; set; }
        public double WorkWeight { get; set; } = 0.2;
        public double FirstExamWeight { get; set; } = 0.3;
        public double SecondExamWeight { get; set; } = 0.3;
        public double FinalExamWeight { get; set; } = 0.2;
    }
}
