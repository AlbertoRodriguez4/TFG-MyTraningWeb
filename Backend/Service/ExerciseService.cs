using AA2_CS.Model.Entities;

public class ExerciseService
{
    private readonly ExerciseRepository _repository;

    public ExerciseService(ExerciseRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Exercise?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Exercise>> SearchAsync(string query)
    {
        return await _repository.SearchAsync(query);
    }

    public async Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup)
    {
        return await _repository.GetByMuscleGroupAsync(muscleGroup);
    }

    public async Task<Exercise> AddAsync(Exercise exercise)
    {
        return await _repository.AddAsync(exercise);
    }

    public async Task<Exercise> UpdateAsync(Exercise exercise)
    {
        return await _repository.UpdateAsync(exercise);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<List<TaskExercise>> GetTaskExercisesAsync(int taskId)
    {
        return await _repository.GetTaskExercisesAsync(taskId);
    }

    public async Task<TaskExercise> AddTaskExerciseAsync(TaskExercise taskExercise)
    {
        return await _repository.AddTaskExerciseAsync(taskExercise);
    }

    public async Task<bool> DeleteTaskExerciseAsync(int taskExerciseId)
    {
        return await _repository.DeleteTaskExerciseAsync(taskExerciseId);
    }
}
