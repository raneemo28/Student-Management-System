using System;
using System.Collections.Generic;
using System.Text;

namespace App.domain.entity
{
    public class Student
    {
        public string FirstName{get; set;}="unKnown";
        public string LastName { get; set; } = "unknown";
        public string? Student_id { get; set; }
        public string? User_id { get; set; }
    }
}
