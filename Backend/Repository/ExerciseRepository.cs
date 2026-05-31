using Microsoft.EntityFrameworkCore;
using AA2_CS.Model.Entities;
using AA2_CS.Database;
using AA2_CS.Service;

public class ExerciseRepository
{
    private readonly AppDbContext _context;
    private readonly ExerciseDbClient _exerciseDbClient;
    private readonly ExerciseImageFallbackService _imageFallback;

    public ExerciseRepository(AppDbContext context, ExerciseDbClient exerciseDbClient, ExerciseImageFallbackService imageFallback)
    {
        _context = context;
        _exerciseDbClient = exerciseDbClient;
        _imageFallback = imageFallback;
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        var apiExercises = await _exerciseDbClient.GetAllExercisesAsync(50);
        if (apiExercises.Count > 0)
            return apiExercises.Select(MapToExercise).ToList();

        return await _context.Exercises.ToListAsync();
    }

    public async Task<Exercise?> GetByIdAsync(int id)
    {
        return await _context.Exercises.FindAsync(id);
    }

    public async Task<List<Exercise>> SearchAsync(string query)
    {
        var apiExercises = await _exerciseDbClient.SearchExercisesAsync(query);
        if (apiExercises.Count > 0)
            return apiExercises.Select(MapToExercise).ToList();

        return await _context.Exercises
            .Where(e => e.Name.Contains(query) || e.MuscleGroup.Contains(query))
            .ToListAsync();
    }

    public async Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup)
    {
        var targetMuscle = ExerciseDbClient.MapMuscleGroupToTarget(muscleGroup) ?? muscleGroup;
        var apiExercises = await _exerciseDbClient.GetExercisesByTargetAsync(targetMuscle);
        if (apiExercises.Count > 0)
            return apiExercises.Select(MapToExercise).ToList();

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
        // Obtener los ejercicios asociados a una tarea específica, ordenados por el índice de orden para mantener la secuencia correcta de ejercicios dentro de la tarea
        return await _context.TaskExercises
            .Where(te => te.TaskId == taskId)
            .Include(te => te.Exercise)
            .OrderBy(te => te.OrderIndex)
            .ToListAsync();
    }

    public async Task<TaskExercise> AddTaskExerciseAsync(TaskExercise taskExercise)
    {
        // Agregar un ejercicio a una tarea específica, lo que permite construir rutinas de entrenamiento personalizadas para los usuarios
        _context.TaskExercises.Add(taskExercise);
        await _context.SaveChangesAsync();
        return taskExercise;
    }

    public async Task<bool> DeleteTaskExerciseAsync(int taskExerciseId)
    {
        // Eliminar un ejercicio de una tarea específica, lo que permite a los usuarios modificar sus rutinas de entrenamiento según sea necesario
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

    private Exercise MapToExercise(ExerciseDbResponse apiEx)
    {
        var id = int.TryParse(apiEx.Id, out var parsedId)
            ? parsedId
            : Math.Abs(apiEx.Name.GetHashCode()) % 1000000 + 1;

        return new Exercise
        {
            Id = id,
            Name = apiEx.Name,
            Description = string.IsNullOrWhiteSpace(apiEx.Description)
                ? $"Ejercicio dirigido a {apiEx.Target}. Zona: {apiEx.BodyPart}."
                : apiEx.Description,
            MuscleGroup = MapBodyPartToCategory(apiEx.BodyPart),
            Difficulty = string.IsNullOrWhiteSpace(apiEx.Difficulty) ? "intermediate" : apiEx.Difficulty.ToLowerInvariant(),
            Equipment = apiEx.Equipment,
            ImageUrl = !string.IsNullOrWhiteSpace(apiEx.GifUrl)
                ? apiEx.GifUrl
                : _imageFallback.GetImageUrl(apiEx.Name),
            VideoUrl = null,
            Instructions = apiEx.Instructions != null && apiEx.Instructions.Count > 0
                ? string.Join("\n", apiEx.Instructions)
                : null,
            CommonMistakes = null,
            BodyPart = apiEx.BodyPart,
            Target = apiEx.Target,
            Category = apiEx.Category,
            SecondaryMuscles = apiEx.SecondaryMuscles ?? new List<string>(),
            InstructionSteps = apiEx.Instructions ?? new List<string>()
        };
    }

    private static string MapBodyPartToCategory(string bodyPart)
    {
        return bodyPart.ToLowerInvariant() switch
        {
            "chest" => "chest",
            "back" or "lower back" or "upper back" or "spine" => "back",
            "shoulders" or "neck" => "shoulders",
            "upper arms" or "lower arms" => "arms",
            "upper legs" or "lower legs" => "legs",
            "waist" => "core",
            "cardio" => "cardio",
            _ => bodyPart
        };
    }
}
