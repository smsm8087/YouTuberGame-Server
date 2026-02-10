using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouTuberGame.Shared.Models
{
    public class PlayerData
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        public long Subscribers { get; set; } = 0;
        public long TotalViews { get; set; } = 0;
        public long ChannelPower { get; set; } = 0;

        public long Gold { get; set; } = 1000;
        public int Gems { get; set; } = 100;
        public int GachaTickets { get; set; } = 10;
        public int ExpChips { get; set; } = 0;

        public int StudioLevel { get; set; } = 1;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
