using AA2_CS.Database;
using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AA2_CS.Repository
{
    public class SubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }
        public int Add(Subscription subscription)
        {
            // Valida que el usuario existe antes de crear la suscripcion. Se agrega la logica para guardar la fecha de inicio y fin en formato UTC, y se simula el proceso de pago (en producción aquí iría la integración con una pasarela de pago).
            var user = _context.Users.FirstOrDefault(u => u.id == subscription.userid);

            if (user == null)
                throw new ArgumentException("User not found.");

            // Simulación de pago 
            subscription.startDate = subscription.startDate.ToUniversalTime();
            subscription.endDate = subscription.endDate.ToUniversalTime();

            _context.Subscriptions.Add(subscription);
            _context.SaveChanges();

            return subscription.id;
        }

        public List<Subscription> FindAllActive()
        {
            // Obtiene todas las suscripciones activas (aquellas que están marcadas como activas y cuya fecha de fin es mayor a la fecha actual). Se filtran las suscripciones para devolver solo las que están activas y no han expirado.
            return _context.Subscriptions
                .Where(s => s.isActive && s.endDate > DateTime.UtcNow)
                .ToList();
        }

        public List<Subscription> FindAll()
        {
            return _context.Subscriptions.ToList();
        }

   
        public Subscription? FindById(int id)
        {
            return _context.Subscriptions.FirstOrDefault(s => s.id == id);
        }

   
        public Subscription? FindByUserId(int userId)
        {
            return _context.Subscriptions
                .FirstOrDefault(s => s.userid == userId && s.isActive && s.endDate > DateTime.UtcNow);
        }

        public List<SubscriptionDTO> FindHistoryByUserId(int userId)
        {
            // Obtiene el historial de suscripciones de un usuario, incluyendo tanto las activas como las expiradas. Se hace un join entre Subscriptions y Users para obtener 
            // la información del usuario junto con cada suscripción, y se ordena por fecha de inicio descendente para mostrar primero las suscripciones más recientes.
            var subscriptions = (from s in _context.Subscriptions
                                 join user in _context.Users on s.userid equals user.id
                                 where s.userid == userId
                                 orderby s.startDate descending
                                 select new SubscriptionDTO
                                 {
                                     SubscriptionId = s.id,
                                     UserId = s.userid,
                                     UserName = user.name,
                                     UserEmail = user.email,
                                     StartDate = s.startDate,
                                     EndDate = s.endDate,
                                     IsActive = s.isActive,
                                     PlanType = s.planType,
                                     MonthlyPrice = s.monthlyPrice,
                                     NextRenewalDate = s.isActive && s.endDate > DateTime.UtcNow ? s.endDate : null
                                 }).ToList();

            return subscriptions;
        }

        public bool HasActiveSubscription(int userId)
        {
            return _context.Subscriptions
                .Any(s => s.userid == userId && s.isActive && s.endDate > DateTime.UtcNow);
        }

        public int RenewSubscription(int userId, int months = 1)
        {
            // Renueva una suscripción extendiendo su fecha de fin. Se busca la suscripción activa del usuario, se simula el proceso de pago (en producción aquí iría la integración con una pasarela de pago), y se extiende la fecha de fin de la suscripción en función del número de meses especificado.
            var subscription = FindByUserId(userId);

            if (subscription == null)
                throw new InvalidOperationException("No hay una suscripción activa para renovar.");

            // Simulación de pago - no se descuenta oro del juego

            // Extender la fecha de fin
            subscription.endDate = subscription.endDate.AddMonths(months);
            subscription.isActive = true;

            _context.Subscriptions.Update(subscription);
            _context.SaveChanges();

            return subscription.id;
        }

        public int CancelSubscription(int subscriptionId)
        {
            // Cancela una suscripción marcándola como inactiva. Se busca la suscripción por su ID, se verifica que exista, y se actualiza su estado a inactiva para cancelarla.
            var subscription = FindById(subscriptionId);
            if (subscription == null)
                return 0;

            subscription.isActive = false;
            _context.Subscriptions.Update(subscription);
            _context.SaveChanges();

            return 1;
        }

  
        public int DeactivateExpiredSubscriptions()
        {
            // Desactiva las suscripciones que han expirado. Se buscan todas las suscripciones que están marcadas como activas pero cuya fecha de fin es menor o igual a la fecha actual, se marcan como inactivas, y se guardan los cambios en la base de datos.
            var expiredSubscriptions = _context.Subscriptions
                .Where(s => s.isActive && s.endDate <= DateTime.UtcNow)
                .ToList();

            foreach (var subscription in expiredSubscriptions)
            {
                subscription.isActive = false;
            }

            if (expiredSubscriptions.Count > 0)
            {
                _context.Subscriptions.UpdateRange(expiredSubscriptions);
                _context.SaveChanges();
            }

            return expiredSubscriptions.Count;
        }

        public SubscriptionDTO? GetActiveSubscriptionDTO(int userId)
        {
            var subscription = (from s in _context.Subscriptions
                                join user in _context.Users on s.userid equals user.id
                                where s.userid == userId && s.isActive && s.endDate > DateTime.UtcNow
                                select new SubscriptionDTO
                                {
                                    SubscriptionId = s.id,
                                    UserId = s.userid,
                                    UserName = user.name,
                                    UserEmail = user.email,
                                    StartDate = s.startDate,
                                    EndDate = s.endDate,
                                    IsActive = s.isActive,
                                    PlanType = s.planType,
                                    MonthlyPrice = s.monthlyPrice,
                                    NextRenewalDate = s.endDate
                                }).FirstOrDefault();

            return subscription;
        }

        public int PurchaseSubscription(int userId)
        {
            var existingSubscription = FindByUserId(userId);

            if (existingSubscription != null)
            {
                throw new InvalidOperationException("El usuario ya tiene una suscripción activa.");
            }

            var subscription = new Subscription
            {
                userid = userId,
                startDate = DateTime.UtcNow,
                endDate = DateTime.UtcNow.AddMonths(1),
                planType = "Premium",
                monthlyPrice = 10.00m,
                isActive = true
            };

            return Add(subscription);
        }
    }
}
