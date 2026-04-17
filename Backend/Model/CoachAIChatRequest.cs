namespace AA2_CS.Model
{
    public class CoachAIChatRequest
    {
        public string Mensaje { get; set; } = string.Empty;
        public string? ContextoUsuario { get; set; }
        public List<CoachAIMessage> Historial { get; set; } = new();
    }

    public class CoachAIMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
