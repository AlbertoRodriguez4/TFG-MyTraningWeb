using AA2_CS.Service;

public class AchievementService
{
    private readonly AchievementRepository _repository;
    private readonly UserService _userService;

    public AchievementService(AchievementRepository repository, UserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<AA2_CS.Model.Achievement>> GetAllAsync() => await _repository.GetAllAsync();
    public async System.Threading.Tasks.Task<AA2_CS.Model.Achievement?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public async System.Threading.Tasks.Task<AA2_CS.Model.Achievement> AddAsync(AA2_CS.Model.Achievement achievement) => await _repository.AddAsync(achievement);
    public async System.Threading.Tasks.Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    public async System.Threading.Tasks.Task<AA2_CS.Model.Achievement?> UpdateAsync(int id, AA2_CS.Model.Achievement achievement) => await _repository.UpdateAsync(id, achievement);

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<AA2_CS.Model.UserAchievement>> GetUserAchievementsAsync(int userId)
        => await _repository.GetUserAchievementsAsync(userId);

    public async System.Threading.Tasks.Task<int> GetUserAchievementCountAsync(int userId)
        => await _repository.GetUserAchievementCountAsync(userId);

    public async System.Threading.Tasks.Task<bool> HasAchievementAsync(int userId, int achievementId)
        => await _repository.HasAchievementAsync(userId, achievementId);

    public async System.Threading.Tasks.Task<AA2_CS.Model.UserAchievement?> UnlockAchievementAsync(int userId, int achievementId)
    {
        if (await HasAchievementAsync(userId, achievementId))
            return null;

        var achievement = await _repository.GetByIdAsync(achievementId);
        if (achievement == null) return null;

        var userAchievement = await _repository.UnlockAchievementAsync(userId, achievementId);

        // Otorgar recompensas
        if (achievement.RewardGold > 0 || achievement.RewardXP > 0)
        {
            var user = _userService.FindById(userId);
            if (user != null)
            {
                user.gold += achievement.RewardGold;
                _userService.AddExperience(userId, achievement.RewardXP);
            }
        }

        return userAchievement;
    }

    public async System.Threading.Tasks.Task<bool> MarkAsSeenAsync(int userAchievementId)
        => await _repository.MarkAsSeenAsync(userAchievementId);

    public async System.Threading.Tasks.Task<int> CountNewAchievementsAsync(int userId)
        => await _repository.CountNewAchievementsAsync(userId);

    public async System.Threading.Tasks.Task CheckAndUnlockAsync(int userId, string requirementType, int currentValue)
    {
        var achievements = await _repository.GetAllAsync();
        foreach (var achievement in achievements.Where(a => a.RequirementType == requirementType))
        {
            if (currentValue >= achievement.RequirementValue && !await HasAchievementAsync(userId, achievement.Id))
            {
                await UnlockAchievementAsync(userId, achievement.Id);
            }
        }
    }
}
