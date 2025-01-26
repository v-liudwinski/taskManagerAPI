using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Exceptions.TaskLists;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskListController : ControllerBase
    {
        private readonly ITaskListService _taskListService;

        public TaskListController(ITaskListService taskListService)
        {
            _taskListService = taskListService;
        }

        [HttpGet("{taskListId}")]
        public async Task<IActionResult> GetTaskList(string taskListId, [FromQuery] string userId)
        {
            try
            {
                var taskList = await _taskListService.GetTaskListAsync(userId, taskListId);
                return Ok(taskList);
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundTaskListException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskLists([FromQuery] string userId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var taskLists = await _taskListService.GetTaskListsAsync(userId, skip, take);
                return Ok(taskLists);
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskList([FromBody] CreateTaskListDto dto)
        {
            try
            {
                var taskList = await _taskListService.CreateTaskListAsync(dto.UserId, dto.Name);
                return CreatedAtAction(nameof(GetTaskList), new { taskListId = taskList.Id }, taskList);
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{taskListId}")]
        public async Task<IActionResult> UpdateTaskList(string taskListId, [FromBody] UpdateTaskListDto dto)
        {
            try
            {
                var taskList = await _taskListService.UpdateTaskListAsync(dto.UserId, taskListId, dto.NewName);
                return Ok(taskList);
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundTaskListException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [HttpPut("add-relation")]
        public async Task<IActionResult> AddUserRelation(string userId, string taskListId)
        {
            try
            {
                await _taskListService.AddUserRelationAsync(userId, taskListId);
                return NoContent();
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundTaskListException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [HttpPut("remove-relation")]
        public async Task<IActionResult> RemoveUserRelation(string userId, string taskListId)
        {
            try
            {
                await _taskListService.RemoveUserRelationAsync(userId, taskListId);
                return NoContent();
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundTaskListException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }


        [HttpDelete("{taskListId}")]
        public async Task<IActionResult> DeleteTaskList(string taskListId, [FromQuery] string userId)
        {
            try
            {
                await _taskListService.DeleteTaskListAsync(userId, taskListId);
                return NoContent();
            }
            catch (InvalidTaskListRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundTaskListException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }
    }
}
