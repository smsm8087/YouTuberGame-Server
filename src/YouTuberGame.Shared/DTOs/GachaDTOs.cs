using YouTuberGame.Shared.Models;

namespace YouTuberGame.Shared.DTOs
{
    public class GachaRequest
    {
        public int DrawCount { get; set; } = 1;
        public bool UseTicket { get; set; } = false;
    }

    public class GachaResult
    {
        public string InstanceId { get; set; } = string.Empty;
        public string CharacterId { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public CharacterRarity Rarity { get; set; }
        public CharacterSpecialty Specialty { get; set; }
        public bool IsNew { get; set; }
    }

    public class GachaResponse
    {
        public bool Success { get; set; }
        public List<GachaResult> Results { get; set; } = new();
        public int RemainingTickets { get; set; }
        public int RemainingGems { get; set; }
        public string? Message { get; set; }
    }
}
