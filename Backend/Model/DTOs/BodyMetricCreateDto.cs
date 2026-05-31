namespace AA2_CS.Model.DTOs
{
    public class BodyMetricCreateDto
    {
        // DTO para crear un nuevo registro de métricas corporales. Incluye campos para peso, altura, porcentaje de grasa corporal, medidas de pecho, cintura, 
        // brazos y muslos, URL de foto de progreso y notas adicionales.
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
