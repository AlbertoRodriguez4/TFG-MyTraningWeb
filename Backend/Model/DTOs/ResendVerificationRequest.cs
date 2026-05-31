namespace AA2_CS.Model.DTOs
{
    // DTO para la solicitud de reenvío de correo de verificación. Incluye el correo electrónico del usuario para el cual se desea reenviar el correo de verificación.
    public class ResendVerificationRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
