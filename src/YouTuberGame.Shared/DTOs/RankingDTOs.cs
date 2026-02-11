namespace YouTuberGame.Shared.DTOs
{
    public class RankingEntry
    {
        public int Rank { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public long Subscribers { get; set; }
        public long ChannelPower { get; set; }
        public bool IsMe { get; set; }
    }

    public class RankingResponse
    {
        public List<RankingEntry> Rankings { get; set; } = new();
        public RankingEntry? MyRanking { get; set; }
        public int TotalPlayers { get; set; }
    }
}
