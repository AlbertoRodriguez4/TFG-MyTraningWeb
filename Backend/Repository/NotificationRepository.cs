using AA2_CS.Model.Entities;

namespace AA2_CS.Repository
{
    public class NotificationRepository
    {
        private readonly NotificationPreferenceRepository _prefRepo;
        private readonly UserRepository _userRepo;
        private readonly TasksRepository _taskRepo;
        private readonly SubscriptionRepository _subscriptionRepo;

        public NotificationRepository(
            NotificationPreferenceRepository prefRepo,
            UserRepository userRepo,
            TasksRepository taskRepo,
            SubscriptionRepository subscriptionRepo)
        {
            _prefRepo = prefRepo;
            _userRepo = userRepo;
            _taskRepo = taskRepo;
            _subscriptionRepo = subscriptionRepo;
        }

        public async System.Threading.Tasks.Task<(bool shouldSend, User? user, int daysSinceLastTask)> ShouldSendInactivityReminder(int userId)
        {
            var pref = await _prefRepo.GetOrCreateAsync(userId);
            if (!pref.inactivityEnabled)
            {
                return (false, null, 0);
            }

            var lastTaskDate = _taskRepo.GetLastCompletedTaskDate(userId);
            if (lastTaskDate == null)
            {
                return (false, null, 0);
            }

            var daysSinceLastTask = (DateTime.UtcNow - lastTaskDate.Value).Days;
            if (daysSinceLastTask < pref.inactivityDays)
            {
                return (false, null, 0);
            }

            var user = _userRepo.FindById(userId);
            if (user == null || string.IsNullOrEmpty(user.email))
            {
                return (false, null, 0);
            }

            return (true, user, daysSinceLastTask);
        }

        public async System.Threading.Tasks.Task<(bool shouldSend, User? user)> ShouldSendRoomActivityNotification(int userId)
        {
            var pref = await _prefRepo.GetOrCreateAsync(userId);
            if (!pref.roomsEnabled)
            {
                return (false, null);
            }

            var user = _userRepo.FindById(userId);
            if (user == null || string.IsNullOrEmpty(user.email))
            {
                return (false, null);
            }

            return (true, user);
        }

        public async System.Threading.Tasks.Task<(bool shouldSend, User? user)> ShouldSendPurchaseReceipt(int userId)
        {
            var pref = await _prefRepo.GetOrCreateAsync(userId);
            if (!pref.purchasesEnabled)
            {
                return (false, null);
            }

            var user = _userRepo.FindById(userId);
            if (user == null || string.IsNullOrEmpty(user.email))
            {
                return (false, null);
            }

            return (true, user);
        }

        public async System.Threading.Tasks.Task<(bool shouldSend, User? user, Subscription? subscription, int daysLeft)> ShouldSendSubscriptionExpiryWarning(int userId)
        {
            var pref = await _prefRepo.GetOrCreateAsync(userId);
            if (!pref.subscriptionExpiryEnabled)
            {
                return (false, null, null, 0);
            }

            var subscription = _subscriptionRepo.FindByUserId(userId);
            if (subscription == null)
            {
                return (false, null, null, 0);
            }

            var daysLeft = (subscription.endDate - DateTime.UtcNow).Days;
            if (daysLeft > 3 || daysLeft < 0)
            {
                return (false, null, null, 0);
            }

            var user = _userRepo.FindById(userId);
            if (user == null || string.IsNullOrEmpty(user.email))
            {
                return (false, null, null, 0);
            }

            return (true, user, subscription, daysLeft);
        }

        public async System.Threading.Tasks.Task<(bool shouldSend, User? user)> ShouldSendSubscriptionPurchasedNotification(int userId)
        {
            var pref = await _prefRepo.GetOrCreateAsync(userId);
            if (!pref.subscriptionExpiryEnabled)
            {
                return (false, null);
            }

            var user = _userRepo.FindById(userId);
            if (user == null || string.IsNullOrEmpty(user.email))
            {
                return (false, null);
            }

            return (true, user);
        }

        public List<User> GetAllUsersWithEmail()
        {
            return _userRepo.FindAll()
                .Where(u => !string.IsNullOrEmpty(u.email))
                .ToList();
        }
    }
}
