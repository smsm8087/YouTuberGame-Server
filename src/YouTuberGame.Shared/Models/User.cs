using System;
using System.ComponentModel.DataAnnotations;

namespace YouTuberGame.Shared.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(50)]
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ChannelName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        // Navigation property
        public PlayerData? PlayerData { get; set; }
    }
}
