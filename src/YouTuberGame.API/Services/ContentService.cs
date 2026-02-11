using Microsoft.EntityFrameworkCore;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Services
{
    public class ContentService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<ContentService> _logger;
        private readonly Random _random = new Random();

        public ContentService(GameDbContext context, ILogger<ContentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 장르별 기본 제작 시간 (초)
        private static int GetBaseProductionTime(ContentGenre genre) => genre switch
        {
            ContentGenre.Vlog => 300,          // 5분
            ContentGenre.Gaming => 600,         // 10분
            ContentGenre.Review => 480,         // 8분
            ContentGenre.Education => 720,      // 12분
            ContentGenre.Entertainment => 540,  // 9분
            _ => 600
        };

        public async Task<StartContentResponse> StartContentAsync(string userId, StartContentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return new StartContentResponse { Success = false, Message = "제목을 입력해주세요." };
                }

                if (request.AssignedCharacterInstanceIds.Count == 0)
                {
                    return new StartContentResponse { Success = false, Message = "최소 1명의 캐릭터를 배치해야 합니다." };
                }

                if (request.AssignedCharacterInstanceIds.Count > 4)
                {
                    return new StartContentResponse { Success = false, Message = "최대 4명까지 배치할 수 있습니다." };
                }

                // 이미 제작 중인 콘텐츠가 있는지 확인
                var producing = await _context.Contents
                    .AnyAsync(c => c.UserId == userId && c.Status == ContentStatus.Producing);
                if (producing)
                {
                    return new StartContentResponse { Success = false, Message = "이미 제작 중인 콘텐츠가 있습니다." };
                }

                // 배치 캐릭터 검증 및 스탯 계산
                var characters = await _context.PlayerCharacters
                    .Include(pc => pc.Character)
                    .Where(pc => pc.UserId == userId && request.AssignedCharacterInstanceIds.Contains(pc.InstanceId))
                    .ToListAsync();

                if (characters.Count != request.AssignedCharacterInstanceIds.Count)
                {
                    return new StartContentResponse { Success = false, Message = "유효하지 않은 캐릭터가 포함되어 있습니다." };
                }

                // 스탯 합산 (레벨 보너스 포함)
                int filmingScore = 0, editingScore = 0, planningScore = 0, designScore = 0;
                foreach (var pc in characters)
                {
                    float levelMultiplier = 1 + (pc.Level - 1) * 0.02f; // 레벨당 2% 증가
                    filmingScore += (int)(pc.Character!.BaseFilming * levelMultiplier);
                    editingScore += (int)(pc.Character.BaseEditing * levelMultiplier);
                    planningScore += (int)(pc.Character.BasePlanning * levelMultiplier);
                    designScore += (int)(pc.Character.BaseDesign * levelMultiplier);
                }

                int totalQuality = filmingScore + editingScore + planningScore + designScore;
                int productionSeconds = GetBaseProductionTime(request.Genre);

                var content = new Content
                {
                    UserId = userId,
                    Title = request.Title,
                    Genre = request.Genre,
                    AssignedCharacterIds = string.Join(",", request.AssignedCharacterInstanceIds),
                    FilmingScore = filmingScore,
                    EditingScore = editingScore,
                    PlanningScore = planningScore,
                    DesignScore = designScore,
                    TotalQuality = totalQuality,
                    ProductionSeconds = productionSeconds
                };

                _context.Contents.Add(content);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Content started: {UserId} - {Title} ({Genre}), Quality: {Quality}",
                    userId, request.Title, request.Genre, totalQuality);

                return new StartContentResponse
                {
                    Success = true,
                    Message = "콘텐츠 제작을 시작했습니다!",
                    Content = MapToResponse(content)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting content");
                return new StartContentResponse { Success = false, Message = "콘텐츠 제작 시작 중 오류가 발생했습니다." };
            }
        }

        public async Task<List<ContentResponse>> GetProducingContentAsync(string userId)
        {
            var contents = await _context.Contents
                .Where(c => c.UserId == userId && c.Status == ContentStatus.Producing)
                .ToListAsync();

            return contents.Select(MapToResponse).ToList();
        }

        public async Task<CompleteContentResponse> CompleteContentAsync(string userId, string contentId)
        {
            try
            {
                var content = await _context.Contents
                    .FirstOrDefaultAsync(c => c.ContentId == contentId && c.UserId == userId);

                if (content == null)
                {
                    return new CompleteContentResponse { Success = false, Message = "콘텐츠를 찾을 수 없습니다." };
                }

                if (content.Status != ContentStatus.Producing)
                {
                    return new CompleteContentResponse { Success = false, Message = "제작 중인 콘텐츠가 아닙니다." };
                }

                // 시간 체크
                var elapsed = (DateTime.UtcNow - content.StartedAt).TotalSeconds;
                if (elapsed < content.ProductionSeconds)
                {
                    int remaining = content.ProductionSeconds - (int)elapsed;
                    return new CompleteContentResponse
                    {
                        Success = false,
                        Message = $"아직 제작 중입니다. 남은 시간: {remaining}초"
                    };
                }

                content.Status = ContentStatus.Completed;
                content.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Content completed: {UserId} - {ContentId}", userId, contentId);

                return new CompleteContentResponse
                {
                    Success = true,
                    Message = "제작이 완료되었습니다!",
                    Content = MapToResponse(content)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing content");
                return new CompleteContentResponse { Success = false, Message = "콘텐츠 완료 중 오류가 발생했습니다." };
            }
        }

        public async Task<UploadContentResponse> UploadContentAsync(string userId, string contentId)
        {
            try
            {
                var content = await _context.Contents
                    .FirstOrDefaultAsync(c => c.ContentId == contentId && c.UserId == userId);

                if (content == null)
                {
                    return new UploadContentResponse { Success = false, Message = "콘텐츠를 찾을 수 없습니다." };
                }

                if (content.Status != ContentStatus.Completed)
                {
                    return new UploadContentResponse { Success = false, Message = "완료된 콘텐츠만 업로드할 수 있습니다." };
                }

                // 조회수/수익 시뮬레이션 (TotalQuality 기반)
                var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);
                if (playerData == null)
                {
                    return new UploadContentResponse { Success = false, Message = "플레이어 데이터를 찾을 수 없습니다." };
                }

                // 기본 조회수: 품질 * 랜덤(10~30) + 구독자 보너스
                long baseViews = content.TotalQuality * _random.Next(10, 31);
                long subscriberBonus = playerData.Subscribers / 10;
                long views = baseViews + subscriberBonus;
                long likes = views * _random.Next(3, 15) / 100; // 좋아요 3~15%
                long revenue = views / 10; // 10뷰당 1골드
                long newSubscribers = views / _random.Next(50, 200); // 조회수 대비 신규 구독자

                content.Status = ContentStatus.Uploaded;
                content.UploadedAt = DateTime.UtcNow;
                content.Views = views;
                content.Likes = likes;
                content.Revenue = revenue;

                // 플레이어 데이터 업데이트
                playerData.Gold += revenue;
                playerData.TotalViews += views;
                playerData.Subscribers += newSubscribers;
                playerData.ChannelPower = playerData.Subscribers + playerData.TotalViews / 100;
                playerData.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Content uploaded: {UserId} - {ContentId}, Views: {Views}, Revenue: {Revenue}",
                    userId, contentId, views, revenue);

                return new UploadContentResponse
                {
                    Success = true,
                    Message = $"업로드 성공! 조회수: {views:N0}",
                    Views = views,
                    Likes = likes,
                    Revenue = revenue,
                    NewSubscribers = newSubscribers,
                    TotalSubscribers = playerData.Subscribers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading content");
                return new UploadContentResponse { Success = false, Message = "업로드 중 오류가 발생했습니다." };
            }
        }

        public async Task<List<ContentResponse>> GetContentHistoryAsync(string userId, int page = 1, int pageSize = 20)
        {
            var contents = await _context.Contents
                .Where(c => c.UserId == userId && c.Status == ContentStatus.Uploaded)
                .OrderByDescending(c => c.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return contents.Select(MapToResponse).ToList();
        }

        private ContentResponse MapToResponse(Content content)
        {
            int remainingSeconds = 0;
            if (content.Status == ContentStatus.Producing)
            {
                var elapsed = (DateTime.UtcNow - content.StartedAt).TotalSeconds;
                remainingSeconds = Math.Max(0, content.ProductionSeconds - (int)elapsed);
            }

            return new ContentResponse
            {
                ContentId = content.ContentId,
                Title = content.Title,
                Genre = content.Genre,
                Status = content.Status,
                FilmingScore = content.FilmingScore,
                EditingScore = content.EditingScore,
                PlanningScore = content.PlanningScore,
                DesignScore = content.DesignScore,
                TotalQuality = content.TotalQuality,
                ProductionSeconds = content.ProductionSeconds,
                RemainingSeconds = remainingSeconds,
                StartedAt = content.StartedAt,
                CompletedAt = content.CompletedAt,
                UploadedAt = content.UploadedAt,
                Views = content.Views,
                Likes = content.Likes,
                Revenue = content.Revenue
            };
        }
    }
}
