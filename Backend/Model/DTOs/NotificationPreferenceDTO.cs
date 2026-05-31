namespace AA2_CS.Model.DTOs
{
    // DTO para representar las preferencias de notificación de un usuario. Incluye opciones para habilitar o deshabilitar notificaciones de inactividad, 
    // notificaciones relacionadas con salas, compras y vencimiento de suscripciones, así como el número de días
    public class NotificationPreferenceDTO
    {
        public int UserId { get; set; }
        public bool InactivityEnabled { get; set; } = true;
        public int InactivityDays { get; set; } = 3;
        public bool RoomsEnabled { get; set; } = true;
        public bool PurchasesEnabled { get; set; } = true;
        public bool SubscriptionExpiryEnabled { get; set; } = true;
    }
}
