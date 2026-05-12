using Microsoft.EntityFrameworkCore;
using AA2_CS.Model;
using AA2_CS.Database;

public class AchievementRepository
{
    private readonly AppDbContext _context;

    public AchievementRepository(AppDbContext context)
    {
        _context = context;
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
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();
    }

    public async Task<int> GetUserAchievementCountAsync(int userId)
    {
        return await _context.UserAchievements.CountAsync(ua => ua.UserId == userId);
    }

    public async Task<bool> HasAchievementAsync(int userId, int achievementId)
    {
        return await _context.UserAchievements.AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
    }

    public async Task<UserAchievement> UnlockAchievementAsync(int userId, int achievementId)
    {
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
}
