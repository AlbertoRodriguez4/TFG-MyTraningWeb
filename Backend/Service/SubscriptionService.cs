using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using AA2_CS.Repository;

namespace AA2_CS.Service
{
    public class SubscriptionService
    {
        private readonly SubscriptionRepository _repository;

        public SubscriptionService(SubscriptionRepository repository)
        {
            _repository = repository;
        }

        public int PurchaseSubscription(int userId)
        {
            return _repository.PurchaseSubscription(userId);
        }

        public int RenewSubscription(int userId)
        {
            return _repository.RenewSubscription(userId, months: 1);
        }

        public int CancelSubscription(int subscriptionId)
        {
            return _repository.CancelSubscription(subscriptionId);
        }

        public SubscriptionDTO? GetActiveSubscription(int userId)
        {
            return _repository.GetActiveSubscriptionDTO(userId);
        }

        public bool HasActiveSubscription(int userId)
        {
            return _repository.HasActiveSubscription(userId);
        }

        public List<SubscriptionDTO> GetSubscriptionHistory(int userId)
        {
            return _repository.FindHistoryByUserId(userId);
        }

        public List<Subscription> GetAllActiveSubscriptions()
        {
            return _repository.FindAllActive();
        }

        public int DeactivateExpiredSubscriptions()
        {
            return _repository.DeactivateExpiredSubscriptions();
        }

        public Subscription? GetById(int id)
        {
            return _repository.FindById(id);
        }

        public List<Subscription> GetAll()
        {
            return _repository.FindAll();
        }
    }
}
