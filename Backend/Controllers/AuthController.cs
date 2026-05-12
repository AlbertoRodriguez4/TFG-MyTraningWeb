using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AA2_CS.Model;
using AA2_CS.Services;

namespace AA2_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                var token = _authService.Login(login.Email, login.Password);

                if (token == null)
                    return Unauthorized("Invalid credentials");

                return Ok(new { token });
            }
            catch (EmailNotVerifiedException ex)
            {
                return StatusCode(403, new { message = ex.Message, requiresEmailVerification = true });
            }
            // El middleware global capturará cualquier otra excepción inesperada
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var (registeredUser, emailSent) = await _authService.RegisterAsync(user);

            if (!emailSent)
            {
                return Ok(new {
                    message = "Usuario registrado correctamente, pero no se pudo enviar el correo de verificación",
                    emailSent = false,
                    userId = registeredUser.id,
                    email = registeredUser.email
                });
            }

            return Ok(new {
                message = "Usuario registrado correctamente. Se ha enviado un código de verificación a tu correo.",
                emailSent = true,
                userId = registeredUser.id,
                email = registeredUser.email
            });
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(new { message = "Email y código son obligatorios" });
            }

            var isVerified = await _authService.VerifyEmailAsync(request.Email, request.Code);

            if (isVerified)
            {
                return Ok(new { message = "Correo verificado exitosamente" });
            }

            return BadRequest(new { message = "Código de verificación inválido o expirado" });
        }

        [HttpPost("resend-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "El email es obligatorio" });
            }

            var emailSent = await _authService.ResendVerificationEmailAsync(request.Email);

            if (emailSent)
            {
                return Ok(new { message = "Se ha reenviado el código de verificación a tu correo" });
            }

            return BadRequest(new { message = "No se pudo reenviar el código de verificación. Es posible que tu correo ya esté verificado" });
        }
    }
}
