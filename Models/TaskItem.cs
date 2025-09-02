namespace TaskManagerAPI.Models;

public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; } // 'required' keyword for non-nullable property
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
}