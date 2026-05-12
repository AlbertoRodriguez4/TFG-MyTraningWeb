using AA2_CS.Model;
using AA2_CS.Service;

public class ExerciseService
{
    private readonly ExerciseRepository _repository;
    private readonly ExerciseDbClient _exerciseDbClient;
    private readonly ExerciseImageFallbackService _imageFallback;

    public ExerciseService(ExerciseRepository repository, ExerciseDbClient exerciseDbClient, ExerciseImageFallbackService imageFallback)
    {
        _repository = repository;
        _exerciseDbClient = exerciseDbClient;
        _imageFallback = imageFallback;
    }

    // Lee ejercicios desde ExerciseDB (externo)
    public async Task<List<Exercise>> GetAllAsync()
    {
        var apiExercises = await _exerciseDbClient.GetAllExercisesAsync(50);
        if (apiExercises.Count > 0)
            return apiExercises.Select(MapToExercise).ToList();

        // Fallback a base de datos si la API falla
        return await _repository.GetAllAsync();
    }

    public async Task<List<Exercise>> SearchAsync(string query)
    {
        var apiExercises = await _exerciseDbClient.SearchExercisesAsync(query);
        if (apiExercises.Count > 0)
            return apiExercises.Select(MapToExercise).ToList();

        return await _repository.SearchAsync(query);
    }

    public async Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup)
    {
        var targetMuscle = ExerciseDbClient.MapMuscleGroupToTarget(muscleGroup) ?? muscleGroup;
        var apiExercises = await _exerciseDbClient.GetExercisesByTargetAsync(targetMuscle);
        if (apiExercises.Count > 0)
            return apiExercises.Select(MapToExercise).ToList();

        return await _repository.GetByMuscleGroupAsync(muscleGroup);
    }

    public async Task<Exercise?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public async Task<Exercise> AddAsync(Exercise exercise) => await _repository.AddAsync(exercise);
    public async Task<Exercise> UpdateAsync(Exercise exercise) => await _repository.UpdateAsync(exercise);
    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    public async Task<List<TaskExercise>> GetTaskExercisesAsync(int taskId) => await _repository.GetTaskExercisesAsync(taskId);
    public async Task<TaskExercise> AddTaskExerciseAsync(TaskExercise taskExercise) => await _repository.AddTaskExerciseAsync(taskExercise);
    public async Task<bool> DeleteTaskExerciseAsync(int taskExerciseId) => await _repository.DeleteTaskExerciseAsync(taskExerciseId);
    public async Task<TaskExercise> UpdateTaskExerciseAsync(TaskExercise taskExercise) => await _repository.UpdateTaskExerciseAsync(taskExercise);

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
