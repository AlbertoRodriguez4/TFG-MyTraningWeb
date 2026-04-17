using System.Security.Claims;
using AA2_CS.Model;
using AA2_CS.Service;
using AA2_CS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AA2_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoachAIController : ControllerBase
    {
        private readonly CoachAIService _coachAIService;
        private readonly SubscriptionService _subscriptionService;

        public CoachAIController(CoachAIService coachAIService, SubscriptionService subscriptionService)
        {
            _coachAIService = coachAIService;
            _subscriptionService = subscriptionService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] CoachAIChatRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Mensaje))
            {
                return BadRequest(new { message = "El mensaje es obligatorio." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Token inválido: no contiene un identificador de usuario válido." });
            }

            if (!_subscriptionService.HasActiveSubscription(userId))
            {
                return StatusCode(403, new { message = "Se requiere una suscripción premium activa para usar CoachAI." });
            }

            try
            {
                var respuesta = await _coachAIService.GenerarRespuestaAsync(request, cancellationToken);
                return Ok(new { reply = respuesta });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
        }
    }
}
