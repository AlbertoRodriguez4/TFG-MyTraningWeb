namespace AA2_CS.Controllers;

using AA2_CS.Model.Entities;
using AA2_CS.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AA2_CS.Extensions;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly TasksService _tasksService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(TasksService tasksService, ILogger<TaskController> logger)
    {
        _tasksService = tasksService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddTask([FromBody] Task task)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized("Token inválido.");
        }

        task.userId = userId.Value;
        task.id = 0;
        var result = _tasksService.Add(task);
        return result > 0 ? Ok(task) : BadRequest("Failed to add task");
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult UpdateTask(int id, [FromBody] Task updatedTask)
    {
        if (id != updatedTask.id)
        {
            return BadRequest("Task ID mismatch.");
        }

        var existingTask = _tasksService.FindById(id);
        if (existingTask == null)
        {
            return NotFound($"Task with ID {id} not found");
        }

        if (!User.IsSelfOrAdmin(existingTask.userId))
        {
            return Forbid();
        }

        var result = _tasksService.Update(updatedTask);
        if (result == 0)
        {
            return NotFound($"Task with ID {updatedTask.id} not found");
        }
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetAllTasks()
    {
        if (!User.IsAdmin())
        {
            return Forbid();
        }

        var tasks = _tasksService.FindAll();
        return Ok(tasks);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public IActionResult GetTasksByUserId(int userId)
    {
        if (!User.IsSelfOrAdmin(userId))
        {
            return Forbid();
        }

        var tasks = _tasksService.FindByUserId(userId);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetTaskById(int id)
    {
        var task = _tasksService.FindById(id);
        if (task == null)
        {
            return NotFound($"Task with ID {id} not found");
        }

        if (!User.IsSelfOrAdmin(task.userId))
        {
            return Forbid();
        }

        return Ok(task);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteTask(int id)
    {
        var task = _tasksService.FindById(id);
        if (task == null)
        {
            return NotFound($"Task with ID {id} not found");
        }

        if (!User.IsSelfOrAdmin(task.userId))
        {
            return Forbid();
        }

        var result = _tasksService.Delete(task);
        return result > 0 ? Ok($"Task with ID {id} deleted successfully") : BadRequest("Failed to delete task");
    }

    [HttpPost("complete/{taskId}")]
    [Authorize]
    public async Task<IActionResult> CompleteTask(int taskId)
    {
        var task = _tasksService.FindById(taskId);
        if (task == null)
        {
            return NotFound("Task not found");
        }

        if (!User.IsSelfOrAdmin(task.userId))
        {
            return Forbid();
        }

        var result = await _tasksService.CompleteTask(taskId);
        if (result == "Task not found")
        {
            return NotFound(result);
        }
        else if (result == "Task is already completed")
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
