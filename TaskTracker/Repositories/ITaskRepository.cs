using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTracker.Models;

namespace TaskTracker.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetTasksForUserAsync(string userId);
        Task<TaskItem?> GetByIdAsync(int id, string userId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task SaveChangesAsync();
    }
}
