namespace App.domain.ReadModels;

public class HomeworkRead
{
    public string Homework_id { get; set; } = string.Empty;
    public string Course_id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string QuestionFileName { get; set; } = string.Empty;
    public string QuestionFileUrl { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int TotalMarks { get; set; }
    public DateTime CreatedAt { get; set; }
}
