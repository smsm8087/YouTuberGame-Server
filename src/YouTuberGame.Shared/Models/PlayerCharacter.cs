using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouTuberGame.Shared.Models
{
    public class PlayerCharacter
    {
        [Key]
        public string InstanceId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CharacterId { get; set; } = string.Empty;

        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Breakthrough { get; set; } = 0;

        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(CharacterId))]
        public Character? Character { get; set; }
    }
}
