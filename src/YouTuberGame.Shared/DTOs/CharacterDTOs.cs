using YouTuberGame.Shared.Models;

namespace YouTuberGame.Shared.DTOs
{
    public class CharacterResponse
    {
        public string CharacterId { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public CharacterRarity Rarity { get; set; }
        public CharacterSpecialty Specialty { get; set; }
        public int BaseFilming { get; set; }
        public int BaseEditing { get; set; }
        public int BasePlanning { get; set; }
        public int BaseDesign { get; set; }
        public string? PassiveSkillDesc { get; set; }
        public float PassiveSkillValue { get; set; }
    }

    public class LevelUpRequest
    {
        public int ExpChipsToUse { get; set; } = 1;
    }

    public class BreakthroughRequest
    {
        public string SacrificeInstanceId { get; set; } = string.Empty;
    }

    public class LevelUpResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int NewLevel { get; set; }
        public int CurrentExp { get; set; }
        public int RequiredExp { get; set; }
        public int MaxLevel { get; set; }
        public int RemainingExpChips { get; set; }
    }

    public class BreakthroughResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int NewBreakthrough { get; set; }
        public int NewMaxLevel { get; set; }
    }
}
