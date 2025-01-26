using MongoDB.Driver;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Models;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskListRepository : ITaskListRepository
    {
        private readonly IMongoDatabase database;
        private const string collectionName = "TaskLists";
        private readonly IMongoCollection<TaskList> taskLists;

        public TaskListRepository(IMongoDatabase database)
        {
            this.database = database;
            taskLists = database.GetCollection<TaskList>(collectionName);
        }

        public async Task CreateAsync(TaskList taskList)
        {
            await taskLists.InsertOneAsync(taskList);
        }

        public async Task<TaskList> GetByIdAsync(string taskListId)
        {
            return await taskLists.Find(t => t.Id == taskListId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TaskList>> GetByUserAsync(string userId, int skip, int take)
        {
            return await taskLists.Find(t => t.OwnerId == userId || t.UserRelations.Any(r => r.UserId == userId))
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
        }

        public async Task UpdateAsync(TaskList taskList)
        {
            await taskLists.ReplaceOneAsync(t => t.Id == taskList.Id, taskList);
        }

        public async Task DeleteAsync(string taskListId)
        {
            await taskLists.DeleteOneAsync(t => t.Id == taskListId);
        }
    }
}
