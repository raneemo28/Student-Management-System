namespace App.Application.DTOs;

public class CourseStudentDto
{
    public string Student_id { get; set; } = string.Empty;
    public string Course_id { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public double? WorkMark { get; set; }
    public double? FirstExamMark { get; set; }
    public double? SecondExamMark { get; set; }
    public double? FinalMark { get; set; }
    public StudentDto? Student { get; set; }
    public CourseDto? Course { get; set; }
}
