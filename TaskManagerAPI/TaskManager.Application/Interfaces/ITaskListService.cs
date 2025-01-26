using TaskManager.Domain.Models;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskListService
    {
        Task<TaskList> CreateTaskListAsync(string userId, string name);
        Task<TaskList> UpdateTaskListAsync(string userId, string taskListId, string newName);
        Task DeleteTaskListAsync(string userId, string taskListId);
        Task<TaskList> GetTaskListAsync(string userId, string taskListId);
        Task<IEnumerable<TaskList>> GetTaskListsAsync(string userId, int skip, int take);
        Task AddUserRelationAsync(string userId, string taskListId);
        Task RemoveUserRelationAsync(string userId, string taskListId);
    }
}
