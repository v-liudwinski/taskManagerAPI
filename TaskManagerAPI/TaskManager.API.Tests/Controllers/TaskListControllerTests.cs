using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Controllers;
using TaskManager.API.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Models;

namespace TaskManager.API.Tests.Controllers
{
    [TestFixture]
    public class TaskListControllerTests
    {
        private Mock<ITaskListService> _taskListServiceMock;
        private TaskListController _controller;

        [SetUp]
        public void Setup()
        {
            _taskListServiceMock = new Mock<ITaskListService>();
            _controller = new TaskListController(_taskListServiceMock.Object);
        }

        [Test]
        public async Task GetTaskList_ReturnsOkResult_WithTaskList()
        {
            // Arrange
            var taskListId = "123";
            var userId = "user1";
            var taskList = new TaskList { Id = taskListId, Name = "Test Task List" };
            _taskListServiceMock
                .Setup(service => service.GetTaskListAsync(userId, taskListId))
                .Returns(Task.FromResult(taskList));

            // Act
            var result = await _controller.GetTaskList(taskListId, userId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(taskList, result.Value);
        }

        [Test]
        public async Task GetTaskLists_ReturnsOkResult_WithTaskLists()
        {
            // Arrange
            var userId = "user1";
            var taskLists = new List<TaskList>
            {
                new TaskList { Id = "1", Name = "Task List 1" },
                new TaskList { Id = "2", Name = "Task List 2" }
            };
            _taskListServiceMock
                .Setup(service => service.GetTaskListsAsync(userId, 0, 10))
                .ReturnsAsync(taskLists);

            // Act
            var result = await _controller.GetTaskLists(userId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(taskLists, result.Value);
        }

        [Test]
        public async Task CreateTaskList_ReturnsCreatedAtActionResult_WithTaskList()
        {
            // Arrange
            var createDto = new CreateTaskListDto { UserId = "user1", Name = "New Task List" };
            var createdTaskList = new TaskList { Id = "123", Name = createDto.Name };
            _taskListServiceMock
                .Setup(service => service.CreateTaskListAsync(createDto.UserId, createDto.Name))
                .ReturnsAsync(createdTaskList);

            // Act
            var result = await _controller.CreateTaskList(createDto) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual(createdTaskList, result.Value);
        }

        [Test]
        public async Task UpdateTaskList_ReturnsOkResult_WithUpdatedTaskList()
        {
            // Arrange
            var taskListId = "123";
            var updateDto = new UpdateTaskListDto { UserId = "user1", NewName = "Updated Task List" };
            var updatedTaskList = new TaskList { Id = taskListId, Name = updateDto.NewName };
            _taskListServiceMock
                .Setup(service => service.UpdateTaskListAsync(updateDto.UserId, taskListId, updateDto.NewName))
                .ReturnsAsync(updatedTaskList);

            // Act
            var result = await _controller.UpdateTaskList(taskListId, updateDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(updatedTaskList, result.Value);
        }

        [Test]
        public async Task AddUserRelation_ReturnsNoContent()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";
            _taskListServiceMock
                .Setup(service => service.AddUserRelationAsync(userId, taskListId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddUserRelation(userId, taskListId) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task RemoveUserRelation_ReturnsNoContent()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";
            _taskListServiceMock
                .Setup(service => service.RemoveUserRelationAsync(userId, taskListId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveUserRelation(userId, taskListId) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteTaskList_ReturnsNoContent()
        {
            // Arrange
            var userId = "user1";
            var taskListId = "123";
            _taskListServiceMock
                .Setup(service => service.DeleteTaskListAsync(userId, taskListId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTaskList(taskListId, userId) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }
    }
}
