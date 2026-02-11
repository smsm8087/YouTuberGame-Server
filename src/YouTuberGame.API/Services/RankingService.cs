using Microsoft.EntityFrameworkCore;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Services
{
    public class RankingService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<RankingService> _logger;

        public RankingService(GameDbContext context, ILogger<RankingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RankingResponse> GetWeeklyRankingAsync(string userId, int topCount = 100)
        {
            try
            {
                // 주간 랭킹: 구독자 수 기준 (실제로는 주간 증가량이어야 하지만, MVP로 단순화)
                var allPlayers = await _context.Users
                    .Join(_context.PlayerData,
                        u => u.UserId,
                        p => p.UserId,
                        (u, p) => new
                        {
                            u.UserId,
                            u.PlayerName,
                            u.ChannelName,
                            p.Subscribers,
                            p.ChannelPower
                        })
                    .OrderByDescending(x => x.Subscribers)
                    .ToListAsync();

                var totalPlayers = allPlayers.Count;

                // Top N
                var topRankings = allPlayers
                    .Take(topCount)
                    .Select((x, index) => new RankingEntry
                    {
                        Rank = index + 1,
                        PlayerName = x.PlayerName,
                        ChannelName = x.ChannelName,
                        Subscribers = x.Subscribers,
                        ChannelPower = x.ChannelPower,
                        IsMe = x.UserId == userId
                    })
                    .ToList();

                // 내 랭킹 찾기
                var myIndex = allPlayers.FindIndex(x => x.UserId == userId);
                RankingEntry? myRanking = null;
                if (myIndex >= 0)
                {
                    var me = allPlayers[myIndex];
                    myRanking = new RankingEntry
                    {
                        Rank = myIndex + 1,
                        PlayerName = me.PlayerName,
                        ChannelName = me.ChannelName,
                        Subscribers = me.Subscribers,
                        ChannelPower = me.ChannelPower,
                        IsMe = true
                    };
                }

                return new RankingResponse
                {
                    Rankings = topRankings,
                    MyRanking = myRanking,
                    TotalPlayers = totalPlayers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly ranking");
                return new RankingResponse();
            }
        }

        public async Task<RankingResponse> GetChannelPowerRankingAsync(string userId, int topCount = 100)
        {
            try
            {
                // 채널 파워 랭킹
                var allPlayers = await _context.Users
                    .Join(_context.PlayerData,
                        u => u.UserId,
                        p => p.UserId,
                        (u, p) => new
                        {
                            u.UserId,
                            u.PlayerName,
                            u.ChannelName,
                            p.Subscribers,
                            p.ChannelPower
                        })
                    .OrderByDescending(x => x.ChannelPower)
                    .ToListAsync();

                var totalPlayers = allPlayers.Count;

                // Top N
                var topRankings = allPlayers
                    .Take(topCount)
                    .Select((x, index) => new RankingEntry
                    {
                        Rank = index + 1,
                        PlayerName = x.PlayerName,
                        ChannelName = x.ChannelName,
                        Subscribers = x.Subscribers,
                        ChannelPower = x.ChannelPower,
                        IsMe = x.UserId == userId
                    })
                    .ToList();

                // 내 랭킹 찾기
                var myIndex = allPlayers.FindIndex(x => x.UserId == userId);
                RankingEntry? myRanking = null;
                if (myIndex >= 0)
                {
                    var me = allPlayers[myIndex];
                    myRanking = new RankingEntry
                    {
                        Rank = myIndex + 1,
                        PlayerName = me.PlayerName,
                        ChannelName = me.ChannelName,
                        Subscribers = me.Subscribers,
                        ChannelPower = me.ChannelPower,
                        IsMe = true
                    };
                }

                return new RankingResponse
                {
                    Rankings = topRankings,
                    MyRanking = myRanking,
                    TotalPlayers = totalPlayers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting channel power ranking");
                return new RankingResponse();
            }
        }
    }
}
