namespace AA2_CS.Model.DTOs
{ // DTO para la solicitud de chat con el Coach AI. Incluye el mensaje del usuario, un contexto opcional sobre el usuario y un historial de mensajes anteriores en la conversación.
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
