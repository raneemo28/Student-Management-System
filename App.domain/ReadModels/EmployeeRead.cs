namespace App.domain.ReadModels;

public class EmployeeRead
{
    public string Employee_id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? User_id { get; set; }
}
