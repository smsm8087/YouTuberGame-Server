using Microsoft.EntityFrameworkCore;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Services
{
    public class AdminService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(GameDbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();

                // DAU: 최근 24시간 이내 로그인한 유저
                var yesterday = DateTime.UtcNow.AddDays(-1);
                var dau = await _context.Users
                    .Where(u => u.LastLogin >= yesterday)
                    .CountAsync();

                var totalGachaDraws = await _context.PlayerCharacters.CountAsync();
                var totalContentUploaded = await _context.Contents
                    .Where(c => c.Status == ContentStatus.Uploaded)
                    .CountAsync();

                return new DashboardData
                {
                    TotalUsers = totalUsers,
                    DailyActiveUsers = dau,
                    TotalGachaDraws = totalGachaDraws,
                    TotalContentUploaded = totalContentUploaded,
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data");
                return new DashboardData();
            }
        }

        public async Task<AdminUserListResponse> GetUsersAsync(string? searchQuery = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var query = _context.Users
                    .Join(_context.PlayerData,
                        u => u.UserId,
                        p => p.UserId,
                        (u, p) => new { User = u, Player = p });

                // 검색
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(x =>
                        x.User.Email.Contains(searchQuery) ||
                        x.User.PlayerName.Contains(searchQuery) ||
                        x.User.ChannelName.Contains(searchQuery));
                }

                var totalCount = await query.CountAsync();

                var users = await query
                    .OrderByDescending(x => x.User.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new AdminUserData
                    {
                        UserId = x.User.UserId,
                        Email = x.User.Email,
                        PlayerName = x.User.PlayerName,
                        ChannelName = x.User.ChannelName,
                        Gold = x.Player.Gold,
                        Gem = x.Player.Gems,
                        Subscribers = x.Player.Subscribers,
                        CreatedAt = x.User.CreatedAt,
                        LastLoginAt = x.User.LastLogin
                    })
                    .ToListAsync();

                return new AdminUserListResponse
                {
                    Users = users,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users list");
                return new AdminUserListResponse();
            }
        }

        public async Task<AdminUserDetailResponse?> GetUserDetailAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return null;

                var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);
                if (playerData == null) return null;

                // 캐릭터 목록
                var characters = await _context.PlayerCharacters
                    .Where(pc => pc.UserId == userId)
                    .Join(_context.Characters,
                        pc => pc.CharacterId,
                        c => c.CharacterId,
                        (pc, c) => new CharacterInstanceDTO
                        {
                            InstanceId = pc.InstanceId,
                            CharacterId = c.CharacterId,
                            CharacterName = c.CharacterName,
                            Rarity = c.Rarity,
                            Level = pc.Level,
                            Experience = pc.Experience,
                            Breakthrough = pc.Breakthrough,
                            Filming = c.BaseFilming,
                            Editing = c.BaseEditing,
                            Planning = c.BasePlanning,
                            Design = c.BaseDesign
                        })
                    .ToListAsync();

                // 콘텐츠 히스토리 (최근 20개)
                var contentHistory = await _context.Contents
                    .Where(c => c.UserId == userId && c.Status == ContentStatus.Uploaded)
                    .OrderByDescending(c => c.UploadedAt)
                    .Take(20)
                    .Select(c => new ContentHistoryItem
                    {
                        ContentId = c.ContentId,
                        Title = c.Title,
                        Genre = c.Genre,
                        TotalQuality = c.TotalQuality,
                        Views = c.Views,
                        Likes = c.Likes,
                        Revenue = c.Revenue,
                        UploadedAt = c.UploadedAt ?? DateTime.MinValue
                    })
                    .ToListAsync();

                return new AdminUserDetailResponse
                {
                    User = new AdminUserData
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        PlayerName = user.PlayerName,
                        ChannelName = user.ChannelName,
                        Gold = playerData.Gold,
                        Gem = playerData.Gems,
                        Subscribers = playerData.Subscribers,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLogin
                    },
                    Characters = characters,
                    ContentHistory = contentHistory
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user detail for {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> UpdateUserCurrencyAsync(string userId, UpdateCurrencyRequest request)
        {
            try
            {
                var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);
                if (playerData == null) return false;

                playerData.Gold += request.Gold;
                playerData.Gems += (int)request.Gem;

                // 음수 방지
                if (playerData.Gold < 0) playerData.Gold = 0;
                if (playerData.Gems < 0) playerData.Gems = 0;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin updated currency for {UserId}: Gold {Gold}, Gem {Gem}, Reason: {Reason}",
                    userId, request.Gold, request.Gem, request.Reason);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating currency for {UserId}", userId);
                return false;
            }
        }

        public async Task<SendRewardResponse> SendRewardAsync(SendRewardRequest request)
        {
            try
            {
                List<PlayerData> targets;

                // 전체 지급 vs 특정 유저
                if (request.TargetUserIds == null || request.TargetUserIds.Count == 0)
                {
                    targets = await _context.PlayerData.ToListAsync();
                }
                else
                {
                    targets = await _context.PlayerData
                        .Where(p => request.TargetUserIds.Contains(p.UserId))
                        .ToListAsync();
                }

                if (targets.Count == 0)
                {
                    return new SendRewardResponse
                    {
                        Success = false,
                        AffectedUsers = 0,
                        Message = "대상 유저가 없습니다."
                    };
                }

                // 보상 지급
                foreach (var player in targets)
                {
                    player.Gold += request.Gold;
                    player.Gems += (int)request.Gem;
                    player.ExpChips += request.ExpChips;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin sent reward to {Count} users: Gold {Gold}, Gem {Gem}, ExpChips {ExpChips}, Reason: {Reason}",
                    targets.Count, request.Gold, request.Gem, request.ExpChips, request.RewardReason);

                return new SendRewardResponse
                {
                    Success = true,
                    AffectedUsers = targets.Count,
                    Message = $"{targets.Count}명에게 보상을 지급했습니다."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reward");
                return new SendRewardResponse
                {
                    Success = false,
                    AffectedUsers = 0,
                    Message = $"오류 발생: {ex.Message}"
                };
            }
        }

        public async Task<GachaStatistics> GetGachaStatisticsAsync()
        {
            try
            {
                var allCharacters = await _context.PlayerCharacters
                    .Join(_context.Characters,
                        pc => pc.CharacterId,
                        c => c.CharacterId,
                        (pc, c) => c.Rarity)
                    .ToListAsync();

                var totalDraws = allCharacters.Count;
                var cCount = allCharacters.Count(r => r == CharacterRarity.C);
                var bCount = allCharacters.Count(r => r == CharacterRarity.B);
                var aCount = allCharacters.Count(r => r == CharacterRarity.A);
                var sCount = allCharacters.Count(r => r == CharacterRarity.S);

                return new GachaStatistics
                {
                    TotalDraws = totalDraws,
                    TotalCRarity = cCount,
                    TotalBRarity = bCount,
                    TotalARarity = aCount,
                    TotalSRarity = sCount,
                    ActualCRate = totalDraws > 0 ? (double)cCount / totalDraws * 100 : 0,
                    ActualBRate = totalDraws > 0 ? (double)bCount / totalDraws * 100 : 0,
                    ActualARate = totalDraws > 0 ? (double)aCount / totalDraws * 100 : 0,
                    ActualSRate = totalDraws > 0 ? (double)sCount / totalDraws * 100 : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gacha statistics");
                return new GachaStatistics();
            }
        }

        public async Task<ContentStatistics> GetContentStatisticsAsync()
        {
            try
            {
                var allContents = await _context.Contents.ToListAsync();
                var uploadedContents = allContents.Where(c => c.Status == ContentStatus.Uploaded).ToList();

                var genreDistribution = allContents
                    .GroupBy(c => c.Genre.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());

                var avgQuality = uploadedContents.Count > 0
                    ? uploadedContents.Average(c => c.TotalQuality)
                    : 0;

                return new ContentStatistics
                {
                    TotalContents = allContents.Count,
                    TotalUploaded = uploadedContents.Count,
                    GenreDistribution = genreDistribution,
                    AverageQuality = avgQuality,
                    TotalViews = uploadedContents.Sum(c => c.Views),
                    TotalRevenue = uploadedContents.Sum(c => c.Revenue)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting content statistics");
                return new ContentStatistics();
            }
        }
    }
}
