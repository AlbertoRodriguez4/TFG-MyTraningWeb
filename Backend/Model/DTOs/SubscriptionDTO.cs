namespace AA2_CS.Model.DTOs
{
    // DTO para representar la información de una suscripción de un usuario. Incluye detalles como el ID de la suscripción, el ID del usuario, el nombre y correo electrónico del usuario, 
    // las fechas de inicio y fin de la suscripción, el estado de la suscripción, el tipo
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PlanType { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public DateTime? NextRenewalDate { get; set; }
    }
}
