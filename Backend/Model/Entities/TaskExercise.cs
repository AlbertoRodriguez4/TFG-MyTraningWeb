namespace AA2_CS.Model.Entities
{
    public class TaskExercise
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int Sets { get; set; } = 3;
        public int Reps { get; set; } = 10;
        public float? Weight { get; set; }
        public int RestSeconds { get; set; } = 60;
        public bool IsCompleted { get; set; } = false;
        public int OrderIndex { get; set; } = 0;
    }
}
