using AA2_CS.Model.DTOs;
using AA2_CS.Repository;

namespace AA2_CS.Service
{
    public class NotificationPreferenceService
    {
        private readonly NotificationPreferenceRepository _repository;

        public NotificationPreferenceService(NotificationPreferenceRepository repository)
        {
            _repository = repository;
        }

        public async System.Threading.Tasks.Task<NotificationPreferenceDTO> GetByUserIdAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async System.Threading.Tasks.Task<NotificationPreferenceDTO> UpdateAsync(int userId, NotificationPreferenceDTO dto)
        {
            return await _repository.UpdateAsync(userId, dto);
        }

        public async System.Threading.Tasks.Task<NotificationPreferenceDTO> ResetDefaultsAsync(int userId)
        {
            return await _repository.ResetDefaultsAsync(userId);
        }
    }
}
