using System.ComponentModel.DataAnnotations;

namespace YouTuberGame.Shared.Models
{
    public enum CharacterRarity
    {
        C = 0,
        B = 1,
        A = 2,
        S = 3
    }

    public enum CharacterSpecialty
    {
        Filming,
        Editing,
        Planning,
        Design
    }

    public class Character
    {
        [Key]
        [MaxLength(50)]
        public string CharacterId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
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
}
