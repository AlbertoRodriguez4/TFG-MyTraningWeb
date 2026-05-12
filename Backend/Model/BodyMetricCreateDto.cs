namespace AA2_CS.Model
{
    public class BodyMetricCreateDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public float? Weight { get; set; }
        public float? Height { get; set; }
        public float? BodyFat { get; set; }
        public float? Chest { get; set; }
        public float? Waist { get; set; }
        public float? Arms { get; set; }
        public float? Thighs { get; set; }
        public string? ProgressPhotoUrl { get; set; }
        public string? Notes { get; set; }
    }
}
