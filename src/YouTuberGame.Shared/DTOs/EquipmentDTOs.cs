using YouTuberGame.Shared.Models;

namespace YouTuberGame.Shared.DTOs
{
    public class EquipmentResponse
    {
        public EquipmentType EquipmentType { get; set; }
        public int Level { get; set; }
        public int UpgradeCost { get; set; }
        public int BonusValue { get; set; }
    }

    public class UpgradeEquipmentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int NewLevel { get; set; }
        public int NextUpgradeCost { get; set; }
        public long RemainingGold { get; set; }
    }
}
