using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AA2_CS.Model.Entities;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExerciseController : ControllerBase
{
    private readonly ExerciseService _service;

    public ExerciseController(ExerciseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var exercises = await _service.GetAllAsync();
        return Ok(exercises);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var exercise = await _service.GetByIdAsync(id);
        if (exercise == null) return NotFound();
        return Ok(exercise);
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> Search(string query)
    {
        var exercises = await _service.SearchAsync(query);
        return Ok(exercises);
    }

    [HttpGet("muscle-group/{muscleGroup}")]
    public async Task<IActionResult> GetByMuscleGroup(string muscleGroup)
    {
        var exercises = await _service.GetByMuscleGroupAsync(muscleGroup);
        return Ok(exercises);
    }

    [HttpPost]
    [Authorize(Roles = "userMaster,userStaff")]
    public async Task<IActionResult> Create([FromBody] Exercise exercise)
    {
        var created = await _service.AddAsync(exercise);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "userMaster,userStaff")]
    public async Task<IActionResult> Update(int id, [FromBody] Exercise exercise)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        exercise.Id = id;
        var updated = await _service.UpdateAsync(exercise);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "userMaster")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { mensaje = "Ejercicio eliminado." });
    }

    [HttpGet("task/{taskId}")]
    [Authorize]
    public async Task<IActionResult> GetTaskExercises(int taskId)
    {
        var taskExercises = await _service.GetTaskExercisesAsync(taskId);
        return Ok(taskExercises);
    }

    [HttpPost("task-exercise")]
    [Authorize]
    public async Task<IActionResult> AddTaskExercise([FromBody] TaskExercise taskExercise)
    {
        var created = await _service.AddTaskExerciseAsync(taskExercise);
        return Ok(created);
    }

    [HttpDelete("task-exercise/{taskExerciseId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTaskExercise(int taskExerciseId)
    {
        var result = await _service.DeleteTaskExerciseAsync(taskExerciseId);
        if (!result) return NotFound();
        return Ok(new { mensaje = "Ejercicio de tarea eliminado." });
    }
}
