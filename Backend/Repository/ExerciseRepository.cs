using Microsoft.EntityFrameworkCore;
using AA2_CS.Model;
using AA2_CS.Database;

public class ExerciseRepository
{
    private readonly AppDbContext _context;

    public ExerciseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        return await _context.Exercises.ToListAsync();
    }

    public async Task<Exercise?> GetByIdAsync(int id)
    {
        return await _context.Exercises.FindAsync(id);
    }

    public async Task<List<Exercise>> SearchAsync(string query)
    {
        return await _context.Exercises
            .Where(e => e.Name.Contains(query) || e.MuscleGroup.Contains(query))
            .ToListAsync();
    }

    public async Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup)
    {
        return await _context.Exercises
            .Where(e => e.MuscleGroup == muscleGroup)
            .ToListAsync();
    }

    public async Task<Exercise> AddAsync(Exercise exercise)
    {
        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();
        return exercise;
    }

    public async Task<Exercise> UpdateAsync(Exercise exercise)
    {
        _context.Exercises.Update(exercise);
        await _context.SaveChangesAsync();
        return exercise;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null) return false;
        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskExercise>> GetTaskExercisesAsync(int taskId)
    {
        return await _context.TaskExercises
            .Where(te => te.TaskId == taskId)
            .Include(te => te.Exercise)
            .OrderBy(te => te.OrderIndex)
            .ToListAsync();
    }

    public async Task<TaskExercise> AddTaskExerciseAsync(TaskExercise taskExercise)
    {
        _context.TaskExercises.Add(taskExercise);
        await _context.SaveChangesAsync();
        return taskExercise;
    }

    public async Task<bool> DeleteTaskExerciseAsync(int taskExerciseId)
    {
        var te = await _context.TaskExercises.FindAsync(taskExerciseId);
        if (te == null) return false;
        _context.TaskExercises.Remove(te);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TaskExercise> UpdateTaskExerciseAsync(TaskExercise taskExercise)
    {
        _context.TaskExercises.Update(taskExercise);
        await _context.SaveChangesAsync();
        return taskExercise;
    }
}
