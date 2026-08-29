using System;
using System.Collections.Generic;
using System.Text;

namespace App.domain.entity
{
    public class Course_student
    {
        public string? Student_id { get; set; }
        public string? Course_id { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public double? WorkMark { get; set; }
        public double? FirstExamMark { get; set; }
        public double? SecondExamMark { get; set; }
        public double? FinalMark { get; set; }
    }
}
