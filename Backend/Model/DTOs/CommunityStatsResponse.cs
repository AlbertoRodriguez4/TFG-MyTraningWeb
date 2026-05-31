namespace AA2_CS.Model.DTOs
{
    // DTO para la respuesta de estadísticas de la comunidad. Incluye el número total de usuarios, el número de usuarios activos hoy y el número total de check-ins 
    // realizados por la comunidad.
    public class CommunityStatsResponse
    {
        public int TotalUsers { get; set; }
        public int ActiveToday { get; set; }
        public int TotalCheckIns { get; set; }
    }
}
