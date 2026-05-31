namespace AA2_CS.Model.Entities
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "beginner";
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string Equipment { get; set; } = "peso corporal";
        public string? Instructions { get; set; }
        public string? CommonMistakes { get; set; }
        public string BodyPart { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string>? SecondaryMuscles { get; set; } = new();
        public List<string>? InstructionSteps { get; set; } = new();

        public Exercise() { }
    }
}
