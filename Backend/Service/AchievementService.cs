using AA2_CS.Model.Entities;

public class AchievementService
{
    private readonly AchievementRepository _repository;

    public AchievementService(AchievementRepository repository)
    {
        _repository = repository;
    }

    public async Task<System.Collections.Generic.List<Achievement>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async System.Threading.Tasks.Task<Achievement?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async System.Threading.Tasks.Task<Achievement> AddAsync(Achievement achievement)
    {
        return await _repository.AddAsync(achievement);
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async System.Threading.Tasks.Task<Achievement?> UpdateAsync(int id, Achievement achievement)
    {
        return await _repository.UpdateAsync(id, achievement);
    }

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<UserAchievement>> GetUserAchievementsAsync(int userId)
    {
        return await _repository.GetUserAchievementsAsync(userId);
    }

    public async System.Threading.Tasks.Task<int> GetUserAchievementCountAsync(int userId)
    {
        return await _repository.GetUserAchievementCountAsync(userId);
    }

    public async System.Threading.Tasks.Task<bool> HasAchievementAsync(int userId, int achievementId)
    {
        return await _repository.HasAchievementAsync(userId, achievementId);
    }

    public async System.Threading.Tasks.Task<UserAchievement?> UnlockAchievementAsync(int userId, int achievementId)
    {
        return await _repository.UnlockWithRewardsAsync(userId, achievementId);
    }

    public async System.Threading.Tasks.Task<bool> MarkAsSeenAsync(int userAchievementId)
    {
        return await _repository.MarkAsSeenAsync(userAchievementId);
    }

    public async System.Threading.Tasks.Task<int> CountNewAchievementsAsync(int userId)
    {
        return await _repository.CountNewAchievementsAsync(userId);
    }

    public async System.Threading.Tasks.Task CheckAndUnlockAsync(int userId, string requirementType, int currentValue)
    {
        await _repository.CheckAndUnlockAsync(userId, requirementType, currentValue);
    }

    public async System.Threading.Tasks.Task EvaluateUserAchievementsAsync(int userId)
    {
        await _repository.EvaluateUserAchievementsAsync(userId);
    }
}
