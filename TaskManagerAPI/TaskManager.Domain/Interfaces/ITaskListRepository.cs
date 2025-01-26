using TaskManager.Domain.Models;

namespace TaskManager.Domain.Interfaces
{
    public interface ITaskListRepository
    {
        Task CreateAsync(TaskList taskList);
        Task<TaskList> GetByIdAsync(string taskListId);
        Task<IEnumerable<TaskList>> GetByUserAsync(string userId, int skip, int take);
        Task UpdateAsync(TaskList taskList);
        Task DeleteAsync(string taskListId);
    }
}
