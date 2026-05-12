using AA2_CS.Repository;

namespace AA2_CS.Service
{
    public class NotificationBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationBackgroundService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Calcular el tiempo hasta las 8:00 UTC del próximo día
                var now = DateTime.UtcNow;
                var nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0, DateTimeKind.Utc);
                if (nextRun <= now)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;
                _logger.LogInformation("Próxima ejecución de notificaciones a las {NextRun} UTC (en {Delay})", nextRun, delay);

                await System.Threading.Tasks.Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                _logger.LogInformation("Ejecutando comprobaciones de notificaciones...");

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                    var userRepo = scope.ServiceProvider.GetRequiredService<UserRepository>();

                    // Comprobación de inactividad: iterar sobre TODOS los usuarios con email
                    var usersWithEmail = userRepo.FindAll()
                        .Where(u => !string.IsNullOrEmpty(u.email))
                        .ToList();
                    _logger.LogInformation("Comprobando inactividad para {Count} usuarios", usersWithEmail.Count);

                    foreach (var user in usersWithEmail)
                    {
                        try
                        {
                            await notificationService.SendInactivityReminderIfNeeded(user.id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al enviar recordatorio de inactividad al usuario {UserId}", user.id);
                        }
                    }

                    // Comprobación de expiración de suscripción: iterar sobre TODOS los usuarios con email
                    _logger.LogInformation("Comprobando expiración de suscripción para {Count} usuarios", usersWithEmail.Count);

                    foreach (var user in usersWithEmail)
                    {
                        try
                        {
                            await notificationService.SendSubscriptionExpiryWarningIfNeeded(user.id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al enviar alerta de expiración al usuario {UserId}", user.id);
                        }
                    }

                    _logger.LogInformation("Comprobaciones de notificaciones completadas.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error general en NotificationBackgroundService");
                }
            }
        }
    }
}