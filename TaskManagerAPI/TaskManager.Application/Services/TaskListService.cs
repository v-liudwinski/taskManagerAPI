using System.Xml.Linq;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Exceptions.TaskLists;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Services
{
    public class TaskListService : ITaskListService
    {
        private readonly ITaskListRepository taskListRepository;

        public TaskListService(ITaskListRepository taskListRepository)
        {
            this.taskListRepository = taskListRepository;
        }

        public async Task<TaskList> CreateTaskListAsync(string userId, string name)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(name))
                throw new InvalidTaskListRequestException($"{nameof(userId)} and {nameof(name)} cannot be null or empty.");

            var taskList = new TaskList
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                UserRelations = new List<UserTaskListRelation>
                {
                    new UserTaskListRelation { UserId = userId, DateAdded = DateTime.UtcNow }
                }
            };
            await taskListRepository.CreateAsync(taskList);
            return taskList;
        }

        public async Task<TaskList> UpdateTaskListAsync(string userId, string taskListId, string newName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskListId) || string.IsNullOrEmpty(newName))
                throw new InvalidTaskListRequestException($"{nameof(userId)}, {nameof(taskListId)} and {nameof(newName)} cannot be null or empty.");

            var taskList = await taskListRepository.GetByIdAsync(taskListId);
            if (taskList == null)
                throw new NotFoundTaskListException("Task list is not found.");
            else if (taskList.OwnerId != userId)
                throw new ForbiddenAccessException("You are not authorized to update this task list.");

            taskList.Name = newName;
            await taskListRepository.UpdateAsync(taskList);
            return taskList;
        }

        public async Task<TaskList> GetTaskListAsync(string userId, string taskListId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskListId))
                throw new InvalidTaskListRequestException($"{nameof(userId)} and {nameof(taskListId)} cannot be null or empty.");

            var taskList = await taskListRepository.GetByIdAsync(taskListId);
            if (taskList == null)
                throw new NotFoundTaskListException("Task list is not found.");
            else if (!taskList.UserRelations.Any(r => r.UserId == userId) && taskList.OwnerId != userId)
                throw new ForbiddenAccessException("You are not authorized to view this task list.");

            return taskList;
        }

        public async Task<IEnumerable<TaskList>> GetTaskListsAsync(string userId, int skip, int take)
        {
            if (string.IsNullOrEmpty(userId) || skip < 0 || take <= 0)
                throw new InvalidTaskListRequestException($"{nameof(userId)} cannot be null or empty, {nameof(skip)} and {nameof(take)} should be grether or equal then 0.");

            var taskLists = await taskListRepository.GetByUserAsync(userId, skip, take);
            return taskLists.OrderByDescending(t => t.CreatedAt);
        }
        
        public async Task AddUserRelationAsync(string userId, string taskListId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskListId))
                throw new InvalidTaskListRequestException($"{nameof(userId)} and {nameof(taskListId)} cannot be null or empty.");

            var taskList = await taskListRepository.GetByIdAsync(taskListId);
            if (taskList == null)
                throw new NotFoundTaskListException("Task list is not found.");
            else if (taskList.OwnerId == userId || taskList.UserRelations.Any(r => r.UserId == userId))
                throw new ForbiddenAccessException("Cannot add user to this task list.");

            taskList.UserRelations.Add(new UserTaskListRelation { UserId = userId, DateAdded = DateTime.UtcNow });
            await taskListRepository.UpdateAsync(taskList);
        }

        public async Task RemoveUserRelationAsync(string userId, string taskListId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskListId))
                throw new InvalidTaskListRequestException($"{nameof(userId)} and {nameof(taskListId)} cannot be null or empty.");

            var taskList = await taskListRepository.GetByIdAsync(taskListId);
            if (taskList == null)
                throw new NotFoundTaskListException("Task list is not found.");
            else if (taskList.OwnerId == userId || !taskList.UserRelations.Any(r => r.UserId == userId))
                throw new ForbiddenAccessException("Cannot remove user from this task list.");

            taskList.UserRelations.RemoveAll(r => r.UserId == userId);
            await taskListRepository.UpdateAsync(taskList);
        }

        public async Task DeleteTaskListAsync(string userId, string taskListId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskListId))
                throw new InvalidTaskListRequestException($"{nameof(userId)} and {nameof(taskListId)} cannot be null or empty.");

            var taskList = await taskListRepository.GetByIdAsync(taskListId);
            if (taskList == null)
                throw new NotFoundTaskListException("Task list is not found.");
            else if (taskList.OwnerId != userId)
                throw new ForbiddenAccessException("You are not authorized to delete this task list.");

            await taskListRepository.DeleteAsync(taskListId);
        }
    }
}
