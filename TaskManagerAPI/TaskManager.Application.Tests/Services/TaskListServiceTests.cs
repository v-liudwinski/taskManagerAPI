using Moq;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Tests.Services
{
    [TestFixture]
    public class TaskListServiceTests
    {
        private Mock<ITaskListRepository> _taskListRepositoryMock;
        private TaskListService _service;

        [SetUp]
        public void Setup()
        {
            _taskListRepositoryMock = new Mock<ITaskListRepository>();
            _service = new TaskListService(_taskListRepositoryMock.Object);
        }

        [Test]
        public async Task CreateTaskListAsync_ValidInputs_ReturnsCreatedTaskList()
        {
            // Arrange
            var userId = "user1";
            var name = "Test Task List";

            // Act
            var result = await _service.CreateTaskListAsync(userId, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(userId, result.OwnerId);
            _taskListRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<TaskList>()), Times.Once);
        }

        [Test]
        public void CreateTaskListAsync_NullOrEmptyInputs_ThrowsArgumentNullException()
        {
            // Arrange
            var userId = "";
            var name = "Test Task List";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateTaskListAsync(userId, name));
        }

        [Test]
        public async Task UpdateTaskListAsync_ValidInputs_ReturnsUpdatedTaskList()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";
            var newName = "Updated Task List";

            var existingTaskList = new TaskList { Id = taskListId, Name = "Old Task List", OwnerId = userId };
            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync(existingTaskList);

            // Act
            var result = await _service.UpdateTaskListAsync(userId, taskListId, newName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newName, result.Name);
            _taskListRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TaskList>()), Times.Once);
        }

        [Test]
        public void UpdateTaskListAsync_TaskListNotFound_ThrowsArgumentNullException()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";
            var newName = "Updated Task List";

            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync((TaskList)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateTaskListAsync(userId, taskListId, newName));
        }

        [Test]
        public void UpdateTaskListAsync_NotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var userId = "user2";
            var taskListId = "123";
            var newName = "Updated Task List";

            var existingTaskList = new TaskList { Id = taskListId, Name = "Old Task List", OwnerId = "user1" };
            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync(existingTaskList);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateTaskListAsync(userId, taskListId, newName));
        }

        [Test]
        public async Task GetTaskListAsync_ValidInputs_ReturnsTaskList()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";

            var taskList = new TaskList
            {
                Id = taskListId,
                Name = "Test Task List",
                OwnerId = userId,
                UserRelations = new List<UserTaskListRelation> { new UserTaskListRelation { UserId = userId } }
            };

            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync(taskList);

            // Act
            var result = await _service.GetTaskListAsync(userId, taskListId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskList, result);
        }

        [Test]
        public void GetTaskListAsync_NotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var userId = "user2";
            var taskListId = "123";

            var taskList = new TaskList
            {
                Id = taskListId,
                Name = "Test Task List",
                OwnerId = "user1",
                UserRelations = new List<UserTaskListRelation>()
            };

            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync(taskList);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetTaskListAsync(userId, taskListId));
        }

        [Test]
        public async Task GetTaskListsAsync_ValidInputs_ReturnsOrderedTaskLists()
        {
            // Arrange
            var userId = "user1";
            var taskLists = new List<TaskList>
            {
                new TaskList { Id = "1", Name = "Task List 1", CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new TaskList { Id = "2", Name = "Task List 2", CreatedAt = DateTime.UtcNow }
            };

            _taskListRepositoryMock
                .Setup(repo => repo.GetByUserAsync(userId, 0, 10))
                .ReturnsAsync(taskLists);

            // Act
            var result = await _service.GetTaskListsAsync(userId, 0, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Task List 2", result.First().Name);
        }

        [Test]
        public async Task AddUserRelationAsync_ValidInputs_AddsRelation()
        {
            // Arrange
            var userId = "user2";
            var taskListId = "123";

            var taskList = new TaskList
            {
                Id = taskListId,
                OwnerId = "user1",
                UserRelations = new List<UserTaskListRelation>()
            };

            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync(taskList);

            // Act
            await _service.AddUserRelationAsync(userId, taskListId);

            // Assert
            _taskListRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<TaskList>(t => t.UserRelations.Any(r => r.UserId == userId))), Times.Once);
        }

        [Test]
        public async Task DeleteTaskListAsync_ValidInputs_DeletesTaskList()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";

            var taskList = new TaskList { Id = taskListId, OwnerId = userId };
            _taskListRepositoryMock
                .Setup(repo => repo.GetByIdAsync(taskListId))
                .ReturnsAsync(taskList);

            // Act
            await _service.DeleteTaskListAsync(userId, taskListId);

            // Assert
            _taskListRepositoryMock.Verify(repo => repo.DeleteAsync(taskListId), Times.Once);
        }
    }
}
