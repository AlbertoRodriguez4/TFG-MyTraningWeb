using AA2_CS.Database;
using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AA2_CS.Repository
{
    public class NotificationPreferenceRepository
    {
        private readonly AppDbContext _context;

        public NotificationPreferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<NotificationPreference> AddAsync(NotificationPreference preference)
        {
            _context.NotificationPreferences.Add(preference);
            await _context.SaveChangesAsync();
            return preference;
        }

        public async System.Threading.Tasks.Task<NotificationPreference?> FindByUserIdAsync(int userId)
        {
            return await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.userid == userId);
        }

        public async System.Threading.Tasks.Task<NotificationPreference> GetOrCreateAsync(int userId)
        {
            // Obtiene las preferencias de notificación para un usuario específico, y si no existen, crea una nueva entrada con valores predeterminados, lo que garantiza que siempre haya un registro de preferencias para cada usuario
            var existing = await FindByUserIdAsync(userId);
            if (existing != null)
                return existing;

            var newPref = new NotificationPreference
            {
                userid = userId,
                inactivityEnabled = true,
                inactivityDays = 3,
                roomsEnabled = true,
                purchasesEnabled = true,
                subscriptionExpiryEnabled = true,
                createdat = DateTime.UtcNow,
                updatedat = DateTime.UtcNow
            };

            return await AddAsync(newPref);
        }

        public async System.Threading.Tasks.Task UpdateAsync(NotificationPreference preference)
        {
            preference.updatedat = DateTime.UtcNow;
            _context.NotificationPreferences.Update(preference);
            await _context.SaveChangesAsync();
        }

        public List<NotificationPreference> GetAllWithInactivityEnabled()
        {
            return _context.NotificationPreferences
                .Where(np => np.inactivityEnabled)
                .ToList();
        }

        public List<NotificationPreference> GetAllWithSubscriptionExpiryEnabled()
        {
            return _context.NotificationPreferences
                .Where(np => np.subscriptionExpiryEnabled)
                .ToList();
        }

        public async System.Threading.Tasks.Task<NotificationPreferenceDTO> GetByUserIdAsync(int userId)
        {
            var pref = await GetOrCreateAsync(userId);
            return MapToDTO(pref);
        }

        public async System.Threading.Tasks.Task<NotificationPreferenceDTO> UpdateAsync(int userId, NotificationPreferenceDTO dto)
        {
            if (dto.InactivityDays < 1) dto.InactivityDays = 1;
            if (dto.InactivityDays > 30) dto.InactivityDays = 30;

            var pref = await GetOrCreateAsync(userId);
            pref.inactivityEnabled = dto.InactivityEnabled;
            pref.inactivityDays = dto.InactivityDays;
            pref.roomsEnabled = dto.RoomsEnabled;
            pref.purchasesEnabled = dto.PurchasesEnabled;
            pref.subscriptionExpiryEnabled = dto.SubscriptionExpiryEnabled;
            await UpdateAsync(pref);
            return MapToDTO(pref);
        }

        public async System.Threading.Tasks.Task<NotificationPreferenceDTO> ResetDefaultsAsync(int userId)
        {
            var pref = await GetOrCreateAsync(userId);
            pref.inactivityEnabled = true;
            pref.inactivityDays = 3;
            pref.roomsEnabled = true;
            pref.purchasesEnabled = true;
            pref.subscriptionExpiryEnabled = true;
            await UpdateAsync(pref);
            return MapToDTO(pref);
        }

        private NotificationPreferenceDTO MapToDTO(NotificationPreference pref)
        {
            return new NotificationPreferenceDTO
            {
                UserId = pref.userid,
                InactivityEnabled = pref.inactivityEnabled,
                InactivityDays = pref.inactivityDays,
                RoomsEnabled = pref.roomsEnabled,
                PurchasesEnabled = pref.purchasesEnabled,
                SubscriptionExpiryEnabled = pref.subscriptionExpiryEnabled
            };
        }
    }
}