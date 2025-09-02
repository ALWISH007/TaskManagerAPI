using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<TaskItem> AddTaskAsync(TaskItem taskItem);
    Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updatedTask);
    Task<bool> DeleteTaskAsync(int id);
}