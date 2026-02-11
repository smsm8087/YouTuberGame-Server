using YouTuberGame.Shared.Models;

namespace YouTuberGame.Shared.DTOs
{
    public class StartContentRequest
    {
        public string Title { get; set; } = string.Empty;
        public ContentGenre Genre { get; set; }
        public List<string> AssignedCharacterInstanceIds { get; set; } = new();
    }

    public class ContentResponse
    {
        public string ContentId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public ContentGenre Genre { get; set; }
        public ContentStatus Status { get; set; }
        public int FilmingScore { get; set; }
        public int EditingScore { get; set; }
        public int PlanningScore { get; set; }
        public int DesignScore { get; set; }
        public int TotalQuality { get; set; }
        public int ProductionSeconds { get; set; }
        public int RemainingSeconds { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? UploadedAt { get; set; }
        public long Views { get; set; }
        public long Likes { get; set; }
        public long Revenue { get; set; }
    }

    public class StartContentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ContentResponse? Content { get; set; }
    }

    public class CompleteContentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ContentResponse? Content { get; set; }
    }

    public class UploadContentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public long Views { get; set; }
        public long Likes { get; set; }
        public long Revenue { get; set; }
        public long NewSubscribers { get; set; }
        public long TotalSubscribers { get; set; }
    }
}
