namespace AA2_CS.Model
{
    public class BodyMetric
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public float? Weight { get; set; } // kg
        public float? Height { get; set; } // cm
        public float? BodyFat { get; set; } // %
        public float? Chest { get; set; } // cm
        public float? Waist { get; set; } // cm
        public float? Arms { get; set; } // cm
        public float? Thighs { get; set; } // cm
        public string? ProgressPhotoUrl { get; set; }
        public string? Notes { get; set; }
    }
}
