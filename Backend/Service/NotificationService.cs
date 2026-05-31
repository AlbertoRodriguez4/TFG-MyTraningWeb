using AA2_CS.Repository;

namespace AA2_CS.Service
{
    public class NotificationService
    {
        private readonly EmailService _emailService;
        private readonly NotificationRepository _notificationRepo;

        public NotificationService(
            EmailService emailService,
            NotificationRepository notificationRepo)
        {
            _emailService = emailService;
            _notificationRepo = notificationRepo;
        }

        public async System.Threading.Tasks.Task SendInactivityReminderIfNeeded(int userId)
        {
            var (shouldSend, user, daysSinceLastTask) = await _notificationRepo.ShouldSendInactivityReminder(userId);

            if (!shouldSend || user == null)
            {
                return;
            }

            await _emailService.SendInactivityReminderEmail(user.email, user.name, daysSinceLastTask);
        }

        public async System.Threading.Tasks.Task SendRoomActivityNotificationIfNeeded(int userId, string roomName, string action)
        {
            var (shouldSend, user) = await _notificationRepo.ShouldSendRoomActivityNotification(userId);

            if (!shouldSend || user == null)
            {
                return;
            }

            await _emailService.SendRoomActivityEmail(user.email, user.name, roomName, action);
        }

        public async System.Threading.Tasks.Task SendPurchaseReceiptIfNeeded(int userId, string itemName, string itemType, int itemBonus, int itemPrice)
        {
            var (shouldSend, user) = await _notificationRepo.ShouldSendPurchaseReceipt(userId);

            if (!shouldSend || user == null)
            {
                return;
            }

            await _emailService.SendPurchaseReceiptEmail(
                user.email, user.name, itemName, itemType, itemBonus, itemPrice, DateTime.UtcNow);
        }

        public async System.Threading.Tasks.Task SendSubscriptionExpiryWarningIfNeeded(int userId)
        {
            var (shouldSend, user, subscription, daysLeft) = await _notificationRepo.ShouldSendSubscriptionExpiryWarning(userId);

            if (!shouldSend || user == null || subscription == null)
            {
                return;
            }

            await _emailService.SendSubscriptionExpiryEmail(user.email, user.name, subscription.endDate, daysLeft);
        }

        public async System.Threading.Tasks.Task SendSubscriptionPurchasedNotificationIfNeeded(int userId, DateTime startDate, DateTime endDate, decimal price)
        {
            var (shouldSend, user) = await _notificationRepo.ShouldSendSubscriptionPurchasedNotification(userId);

            if (!shouldSend || user == null)
            {
                return;
            }

            await _emailService.SendSubscriptionPurchasedEmail(user.email, user.name, startDate, endDate, price);
        }
    }
}
