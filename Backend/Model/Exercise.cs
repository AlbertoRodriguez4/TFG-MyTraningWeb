namespace AA2_CS.Model
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty; // pecho, piernas, espalda, hombros, brazos, core, cardio
        public string Difficulty { get; set; } = "beginner"; // beginner, intermediate, advanced
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string Equipment { get; set; } = "peso corporal"; // barra, mancuernas, maquina, peso corporal, bandas, kettlebell
        public string? Instructions { get; set; }
        public string? CommonMistakes { get; set; }

        // Campos de ExerciseDB API
        public string BodyPart { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string>? SecondaryMuscles { get; set; } = new();
        public List<string>? InstructionSteps { get; set; } = new();

        public Exercise() { }
    }
}
