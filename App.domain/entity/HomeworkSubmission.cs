namespace App.domain.entity;

public class HomeworkSubmission
{
    public string Submission_id { get; set; } = string.Empty;
    public string Homework_id { get; set; } = string.Empty;
    public string Student_id { get; set; } = string.Empty;
    public string SolutionFileName { get; set; } = string.Empty;
    public string SolutionFileUrl { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public double? Mark { get; set; }
}
