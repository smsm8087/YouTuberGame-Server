using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouTuberGame.Shared.Models
{
    public enum EquipmentType
    {
        Camera = 0,     // 카메라
        Microphone = 1, // 마이크
        Light = 2,      // 조명
        PC = 3          // 컴퓨터
    }

    /// <summary>
    /// 플레이어 장비 (유저당 4종류)
    /// </summary>
    public class PlayerEquipment
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public EquipmentType EquipmentType { get; set; }

        public int Level { get; set; } = 1;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
