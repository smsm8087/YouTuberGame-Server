namespace YouTuberGame.Shared.DTOs
{
    // Dashboard
    public class DashboardData
    {
        public int TotalUsers { get; set; }
        public int DailyActiveUsers { get; set; }
        public long TotalGachaDraws { get; set; }
        public long TotalContentUploaded { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // User Management
    public class AdminUserListResponse
    {
        public List<AdminUserData> Users { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class AdminUserData
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public long Gold { get; set; }
        public long Gem { get; set; }
        public long Subscribers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
    }

    public class AdminUserDetailResponse
    {
        public AdminUserData User { get; set; } = new();
        public List<CharacterInstanceDTO> Characters { get; set; } = new();
        public List<ContentHistoryItem> ContentHistory { get; set; } = new();
    }

    public class UpdateCurrencyRequest
    {
        public long Gold { get; set; }
        public long Gem { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class SendRewardRequest
    {
        public List<string> TargetUserIds { get; set; } = new(); // 빈 리스트 = 전체 지급
        public long Gold { get; set; }
        public long Gem { get; set; }
        public int ExpChips { get; set; }
        public string RewardReason { get; set; } = string.Empty;
    }

    public class SendRewardResponse
    {
        public bool Success { get; set; }
        public int AffectedUsers { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // Statistics
    public class GachaStatistics
    {
        public long TotalDraws { get; set; }
        public int TotalCRarity { get; set; }
        public int TotalBRarity { get; set; }
        public int TotalARarity { get; set; }
        public int TotalSRarity { get; set; }
        public double ActualCRate { get; set; }
        public double ActualBRate { get; set; }
        public double ActualARate { get; set; }
        public double ActualSRate { get; set; }
    }

    public class ContentStatistics
    {
        public long TotalContents { get; set; }
        public long TotalUploaded { get; set; }
        public Dictionary<string, int> GenreDistribution { get; set; } = new();
        public double AverageQuality { get; set; }
        public long TotalViews { get; set; }
        public long TotalRevenue { get; set; }
    }
}
