using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouTuberGame.Shared.Models
{
    public enum ContentGenre
    {
        Gaming = 0,
        Vlog = 1,
        Review = 2,
        Education = 3,
        Entertainment = 4
    }

    public enum ContentStatus
    {
        Producing = 0,
        Completed = 1,
        Uploaded = 2
    }

    public class Content
    {
        [Key]
        public string ContentId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public ContentGenre Genre { get; set; }
        public ContentStatus Status { get; set; } = ContentStatus.Producing;

        // 배치된 캐릭터 ID (콤마 구분)
        [MaxLength(500)]
        public string AssignedCharacterIds { get; set; } = string.Empty;

        // 제작 품질 점수
        public int FilmingScore { get; set; }
        public int EditingScore { get; set; }
        public int PlanningScore { get; set; }
        public int DesignScore { get; set; }
        public int TotalQuality { get; set; }

        // 제작 시간
        public int ProductionSeconds { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? UploadedAt { get; set; }

        // 업로드 결과
        public long Views { get; set; }
        public long Likes { get; set; }
        public long Revenue { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
