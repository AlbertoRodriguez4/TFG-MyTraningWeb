namespace AA2_CS.Model
{
    public class UserAchievement
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int AchievementId { get; set; }
        public Achievement Achievement { get; set; } = null!;
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
        public bool IsNew { get; set; } = true;
    }
}
