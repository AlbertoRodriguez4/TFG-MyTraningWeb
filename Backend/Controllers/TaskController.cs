namespace AA2_CS.Controllers;

using AA2_CS.Model;
using AA2_CS.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly TasksService _tasksService;

    public TaskController(TasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpPost]
    public IActionResult AddTask([FromBody] Task task)
    {
        try
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized("Token inválido.");
            }

            task.userId = userId;
            var result = _tasksService.Add(task);
            return result > 0 ? Ok(task) : BadRequest("Failed to add task");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, [FromBody] Task updatedTask)
    {
        try
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

            if (!CanAccessUserResource(existingTask.userId))
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet]
    public IActionResult GetAllTasks()
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        try
        {
            var tasks = _tasksService.FindAll();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("user/{userId}")]
    public IActionResult GetTasksByUserId(int userId)
    {
        if (!CanAccessUserResource(userId))
        {
            return Forbid();
        }

        try
        {
            var tasks = _tasksService.FindByUserId(userId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("{id}")]
    public IActionResult GetTaskById(int id)
    {
        try
        {
            var task = _tasksService.FindById(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found");
            }

            if (!CanAccessUserResource(task.userId))
            {
                return Forbid();
            }

            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        try
        {
            var task = _tasksService.FindById(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found");
            }

            if (!CanAccessUserResource(task.userId))
            {
                return Forbid();
            }

            var result = _tasksService.Delete(task);
            return result > 0 ? Ok($"Task with ID {id} deleted successfully") : BadRequest("Failed to delete task");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpPost("complete/{taskId}")]
    public IActionResult CompleteTask(int taskId)
    {
        try
        {
            var task = _tasksService.FindById(taskId);
            if (task == null)
            {
                return NotFound("Task not found");
            }

            if (!CanAccessUserResource(task.userId))
            {
                return Forbid();
            }

            var result = _tasksService.CompleteTask(taskId);
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }

    private bool IsAdmin()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value == Roles.userMaster;
    }

    private bool CanAccessUserResource(int userId)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return false;
        }

        return currentUserId == userId || IsAdmin();
    }
}
