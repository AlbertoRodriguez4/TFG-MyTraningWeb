namespace AA2_CS.Model.Entities
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Category { get; set; } = "general";
        public string RequirementType { get; set; } = string.Empty;
        public int RequirementValue { get; set; }
        public int RewardGold { get; set; } = 0;
        public int RewardXP { get; set; } = 0;
        public string? RewardItemId { get; set; }
        public bool IsSecret { get; set; } = false;

        public Achievement() { }
    }
}
