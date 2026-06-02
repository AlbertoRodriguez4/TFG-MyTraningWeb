using Microsoft.EntityFrameworkCore;
using AA2_CS.Model.Entities;
using AA2_CS.Database;
using AA2_CS.Repository;

public class AchievementRepository
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;

    public AchievementRepository(AppDbContext context, UserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<List<Achievement>> GetAllAsync()
    {
        return await _context.Achievements.ToListAsync();
    }

    public async Task<Achievement?> GetByIdAsync(int id)
    {
        return await _context.Achievements.FindAsync(id);
    }

    public async Task<Achievement> AddAsync(Achievement achievement)
    {
        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();
        return achievement;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var achievement = await _context.Achievements.FindAsync(id);
        if (achievement == null) return false;
        _context.Achievements.Remove(achievement);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Achievement?> UpdateAsync(int id, Achievement updated)
    {
        var achievement = await _context.Achievements.FindAsync(id);
        if (achievement == null) return null;

        achievement.Name = updated.Name;
        achievement.Description = updated.Description;
        achievement.IconUrl = updated.IconUrl;
        achievement.Category = updated.Category;
        achievement.RequirementType = updated.RequirementType;
        achievement.RequirementValue = updated.RequirementValue;
        achievement.RewardGold = updated.RewardGold;
        achievement.RewardXP = updated.RewardXP;
        achievement.RewardItemId = updated.RewardItemId;
        achievement.IsSecret = updated.IsSecret;

        await _context.SaveChangesAsync();
        return achievement;
    }

    public async Task<List<UserAchievement>> GetUserAchievementsAsync(int userId)
    {
        // Obtener los logros del usuario, incluyendo los detalles del logro, ordenados por fecha de desbloqueo (más recientes primero)
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();
    }

    public async Task<int> GetUserAchievementCountAsync(int userId)
    {
        return await _context.UserAchievements.CountAsync(ua => ua.UserId == userId); // Contar el número total de logros desbloqueados por el usuario
    }

    public async Task<bool> HasAchievementAsync(int userId, int achievementId)
    {
        return await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId); // Verificar si el usuario ya tiene un logro específico
    }

    public async Task<UserAchievement> UnlockAchievementAsync(int userId, int achievementId)
    {
        // Verificar si el usuario ya tiene el logro antes de intentar desbloquearlo, para evitar duplicados y otorgar recompensas múltiples por el mismo logro
        if (await HasAchievementAsync(userId, achievementId))
            throw new InvalidOperationException("El usuario ya tiene este logro.");

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow,
            IsNew = true
        };

        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();
        return userAchievement;
    }

    public async Task<bool> MarkAsSeenAsync(int userAchievementId)
    {
        // Marcar un logro como visto por el usuario, lo que se puede usar para mostrar notificaciones solo para logros nuevos y evitar mostrar repetidamente los mismos logros como nuevos
        var ua = await _context.UserAchievements.FindAsync(userAchievementId);
        if (ua == null) return false;
        ua.IsNew = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountNewAchievementsAsync(int userId)
    {
        return await _context.UserAchievements.CountAsync(ua => ua.UserId == userId && ua.IsNew);
    }

    public async System.Threading.Tasks.Task<UserAchievement?> UnlockWithRewardsAsync(int userId, int achievementId)
    {
        if (await HasAchievementAsync(userId, achievementId))
            return null;

        var achievement = await GetByIdAsync(achievementId);
        if (achievement == null) return null;

        var userAchievement = await UnlockAchievementAsync(userId, achievementId);

        if (achievement.RewardGold > 0 || achievement.RewardXP > 0)
        {
            var user = _userRepository.FindById(userId);
            if (user != null)
            {
                user.gold += achievement.RewardGold;
                _userRepository.Update(user);
                _userRepository.AddExperience(userId, achievement.RewardXP);
            }
        }

        return userAchievement;
    }

    public async System.Threading.Tasks.Task EvaluateUserAchievementsAsync(int userId)
    {
        var user = _userRepository.FindById(userId);
        if (user == null) return;

        var achievements = await GetAllAsync();
        var userPurchases = await _context.Purchases
            .Include(p => p.Item)
            .Where(p => p.userid == userId)
            .ToListAsync();
        
        var strengthItemsCount = userPurchases.Count(p => p.Item != null && p.Item.type.Equals("Strength", StringComparison.OrdinalIgnoreCase));
        var enduranceItemsCount = userPurchases.Count(p => p.Item != null && p.Item.type.Equals("Endurance", StringComparison.OrdinalIgnoreCase));
        var totalItemsCount = userPurchases.Count;
        var completedTasksCount = await _context.Tasks.CountAsync(t => t.userId == userId && t.iscompleted);

        foreach (var achievement in achievements)
        {
            if (await HasAchievementAsync(userId, achievement.Id)) continue;

            bool criteriaMet = false;
            switch (achievement.RequirementType?.ToUpper())
            {
                case "PLAYER_LEVEL":
                case "LEVEL_REACHED":
                    criteriaMet = user.level >= achievement.RequirementValue;
                    break;
                case "TOTAL_GOLD":
                case "GOLD_EARNED":
                    criteriaMet = user.gold >= achievement.RequirementValue;
                    break;
                case "ITEMS_COUNT":
                    criteriaMet = totalItemsCount >= achievement.RequirementValue;
                    break;
                case "STRENGTH_COUNT":
                    criteriaMet = strengthItemsCount >= achievement.RequirementValue;
                    break;
                case "ENDURANCE_COUNT":
                    criteriaMet = enduranceItemsCount >= achievement.RequirementValue;
                    break;
                case "SPECIFIC_ITEM":
                    criteriaMet = userPurchases.Any(p => p.itemid == achievement.RequirementValue);
                    break;
                case "STRENGTH_REACHED":
                    criteriaMet = user.strength >= achievement.RequirementValue;
                    break;
                case "ENDURANCE_REACHED":
                    criteriaMet = user.endurance >= achievement.RequirementValue;
                    break;
                case "TASKS_COMPLETED":
                    criteriaMet = completedTasksCount >= achievement.RequirementValue;
                    break;
                case "STREAK_DAYS":
                    criteriaMet = user.consistencystreak >= achievement.RequirementValue;
                    break;
            }

            if (criteriaMet)
            {
                await UnlockWithRewardsAsync(userId, achievement.Id);
            }
        }
    }
}
