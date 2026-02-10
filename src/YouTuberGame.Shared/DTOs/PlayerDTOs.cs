namespace YouTuberGame.Shared.DTOs
{
    public class PlayerDataResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;

        public long Subscribers { get; set; }
        public long TotalViews { get; set; }
        public long ChannelPower { get; set; }

        public long Gold { get; set; }
        public int Gems { get; set; }
        public int GachaTickets { get; set; }
        public int ExpChips { get; set; }

        public int StudioLevel { get; set; }
    }

    public class UpdatePlayerDataRequest
    {
        public long? Gold { get; set; }
        public int? Gems { get; set; }
        public long? Subscribers { get; set; }
        public int? StudioLevel { get; set; }
    }
}
