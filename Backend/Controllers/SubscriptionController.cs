using Microsoft.AspNetCore.Mvc;
using AA2_CS.Model.Entities;
using AA2_CS.Model.Common;
using AA2_CS.Service;
using AA2_CS.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AA2_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionController(SubscriptionService subscriptionService, IServiceProvider serviceProvider)
        {
            _subscriptionService = subscriptionService;
            _serviceProvider = serviceProvider;
        }

   
        [HttpGet]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult GetAllActiveSubscriptions()
        {
            try
            {
                var subscriptions = _subscriptionService.GetAllActiveSubscriptions();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener suscripciones: {ex.Message}");
            }
        }


        [HttpGet("all")]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult GetAllSubscriptions()
        {
            try
            {
                var subscriptions = _subscriptionService.GetAll();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener suscripciones: {ex.Message}");
            }
        }


        [HttpGet("my-subscription")]
        [Authorize]
        public IActionResult GetMySubscription()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Token inválido: No contiene ID de usuario.");
                }

                int userId = int.Parse(userIdString);
                var subscription = _subscriptionService.GetActiveSubscription(userId);

                if (subscription == null)
                {
                    return NotFound(new { message = "No tienes una suscripción activa." });
                }

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener suscripción: {ex.Message}");
            }
        }


        [HttpGet("my-history")]
        [Authorize]
        public IActionResult GetMySubscriptionHistory()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Token inválido: No contiene ID de usuario.");
                }

                int userId = int.Parse(userIdString);
                var history = _subscriptionService.GetSubscriptionHistory(userId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener historial: {ex.Message}");
            }
        }

   
        [HttpGet("check")]
        [Authorize]
        public IActionResult CheckActiveSubscription()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Token inválido: No contiene ID de usuario.");
                }

                int userId = int.Parse(userIdString);
                bool hasActive = _subscriptionService.HasActiveSubscription(userId);

                return Ok(new { hasActiveSubscription = hasActive });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al verificar suscripción: {ex.Message}");
            }
        }

        [HttpPost("purchase")]
        [Authorize]
        public IActionResult PurchaseSubscription()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Token inválido: No contiene ID de usuario.");
                }

                int userId = int.Parse(userIdString);
                var subscriptionId = _subscriptionService.PurchaseSubscription(userId);
                var subscription = _subscriptionService.GetById(subscriptionId);

                // Enviar confirmación de compra de suscripción por email (fire-and-forget)
                if (subscription != null)
                {
                    _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                            await notificationService.SendSubscriptionPurchasedNotificationIfNeeded(
                                userId, subscription.startDate, subscription.endDate, subscription.monthlyPrice);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }

                return Ok(new {
                    id = subscriptionId,
                    message = "Suscripción Premium comprada exitosamente. ¡Disfruta de los beneficios!"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }


        [HttpPost("renew")]
        [Authorize]
        public IActionResult RenewSubscription()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Token inválido: No contiene ID de usuario.");
                }

                int userId = int.Parse(userIdString);
                var subscriptionId = _subscriptionService.RenewSubscription(userId);

                return Ok(new {
                    id = subscriptionId,
                    message = "Suscripción Premium renovada exitosamente por un mes más."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult CancelSubscription(int id)
        {
            try
            {
                // Verificar si es admin o si la suscripción pertenece al usuario
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Token inválido: No contiene ID de usuario.");
                }

                int userId = int.Parse(userIdString);
                var subscription = _subscriptionService.GetById(id);

                if (subscription == null)
                {
                    return NotFound();
                }

                // Solo el dueño de la suscripción o un admin puede cancelarla
                if (subscription.userid != userId && userRole != Roles.userMaster)
                {
                    return Forbid();
                }

                var result = _subscriptionService.CancelSubscription(id);

                if (result > 0)
                {
                    return Ok(new { message = "Suscripción cancelada exitosamente." });
                }

                return BadRequest("No se pudo cancelar la suscripción.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cancelar suscripción: {ex.Message}");
            }
        }

        [HttpPost("deactivate-expired")]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult DeactivateExpiredSubscriptions()
        {
            try
            {
                var count = _subscriptionService.DeactivateExpiredSubscriptions();
                return Ok(new { message = $"{count} suscripciones expiradas han sido desactivadas." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al desactivar suscripciones: {ex.Message}");
            }
        }
    }
}
