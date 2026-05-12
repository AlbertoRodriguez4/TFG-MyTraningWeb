namespace AA2_CS.Model
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Category { get; set; } = "general"; // strength, endurance, consistency, social, exploration
        public string RequirementType { get; set; } = string.Empty; // tasks_completed, streak_days, gold_earned, level_reached, gyms_searched, rooms_joined
        public int RequirementValue { get; set; }
        public int RewardGold { get; set; } = 0;
        public int RewardXP { get; set; } = 0;
        public string? RewardItemId { get; set; }
        public bool IsSecret { get; set; } = false; // logros ocultos

        public Achievement() { }
    }
}
