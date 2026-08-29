namespace App.domain.entity;

public class HomeworkQuestionMark
{
    public string QuestionMark_id { get; set; } = string.Empty;
    public string Submission_id { get; set; } = string.Empty;
    public int QuestionNumber { get; set; }
    public string QuestionDescription { get; set; } = string.Empty;
    public double MaxMarks { get; set; }
    public double? ObtainedMarks { get; set; }
}
