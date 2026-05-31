namespace AA2_CS.Model.DTOs
{
    public class ChangePasswordRequest
    {
        // DTO para la solicitud de cambio de contraseña. Incluye la contraseña actual y la nueva contraseña.
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
