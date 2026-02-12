using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Services
{
    /// <summary>
    /// 마스터 데이터 서비스
    /// 모든 게임 밸런스 값을 한곳에서 관리
    /// 추후 DB 또는 JSON 파일에서 로드하도록 확장 가능
    /// </summary>
    public class MasterDataService
    {
        // 버전이 바뀌면 클라이언트가 새로 다운로드
        private const int CURRENT_VERSION = 1;

        private readonly ILogger<MasterDataService> _logger;

        public MasterDataService(ILogger<MasterDataService> logger)
        {
            _logger = logger;
        }

        public int GetVersion() => CURRENT_VERSION;

        public MasterDataResponse GetMasterData()
        {
            return new MasterDataResponse
            {
                Version = CURRENT_VERSION,
                Gacha = GetGachaBalance(),
                Character = GetCharacterBalance(),
                Content = GetContentBalance(),
                Equipment = GetEquipmentBalance(),
                PlayerStart = GetPlayerStart(),
                Milestones = GetMilestones()
            };
        }

        // ───── 가챠 ─────
        private GachaBalanceData GetGachaBalance()
        {
            return new GachaBalanceData
            {
                GemCostPerDraw = 100,
                MaxDrawCount = 10,
                RarityProbability = new List<KeyValueEntry>
                {
                    new() { Key = "S", Value = 5 },
                    new() { Key = "A", Value = 15 },
                    new() { Key = "B", Value = 30 },
                    new() { Key = "C", Value = 50 }
                }
            };
        }

        // ───── 캐릭터 ─────
        private CharacterBalanceData GetCharacterBalance()
        {
            return new CharacterBalanceData
            {
                MaxLevelByRarity = new List<KeyValueEntry>
                {
                    new() { Key = "C", Value = 30 },
                    new() { Key = "B", Value = 40 },
                    new() { Key = "A", Value = 50 },
                    new() { Key = "S", Value = 60 }
                },
                BreakthroughLevelBonus = 10,
                ExpPerChip = 100,
                LevelStatMultiplier = 0.02f
            };
        }

        // ───── 콘텐츠 ─────
        private ContentBalanceData GetContentBalance()
        {
            return new ContentBalanceData
            {
                ProductionTimeByGenre = new List<KeyValueEntry>
                {
                    new() { Key = "Vlog", Value = 300 },
                    new() { Key = "Gaming", Value = 600 },
                    new() { Key = "Review", Value = 480 },
                    new() { Key = "Education", Value = 720 },
                    new() { Key = "Entertainment", Value = 540 }
                },
                ViewsPerQualityMin = 10,
                ViewsPerQualityMax = 30,
                LikesPercentMin = 3,
                LikesPercentMax = 15,
                ViewsPerGold = 10,
                SubscriberPerViewsMin = 50,
                SubscriberPerViewsMax = 200
            };
        }

        // ───── 장비 ─────
        private EquipmentBalanceData GetEquipmentBalance()
        {
            return new EquipmentBalanceData
            {
                MaxLevel = 10,
                CostPerLevel = 500,
                BonusPerLevel = 5
            };
        }

        // ───── 신규 유저 시작 데이터 ─────
        private PlayerStartData GetPlayerStart()
        {
            return new PlayerStartData
            {
                StartGold = 1000,
                StartGems = 100,
                StartTickets = 10,
                StartExpChips = 0
            };
        }

        // ───── 마일스톤 ─────
        private List<MilestoneData> GetMilestones()
        {
            return new List<MilestoneData>
            {
                new() { RequiredSubscribers = 0, UnlockType = "Genre", UnlockValue = "Vlog", Description = "브이로그 해금 (시작)" },
                new() { RequiredSubscribers = 1000, UnlockType = "Genre", UnlockValue = "Gaming", Description = "게임 장르 해금" },
                new() { RequiredSubscribers = 5000, UnlockType = "Genre", UnlockValue = "Review", Description = "리뷰 장르 해금" },
                new() { RequiredSubscribers = 20000, UnlockType = "Genre", UnlockValue = "Education", Description = "교육 장르 해금" },
                new() { RequiredSubscribers = 50000, UnlockType = "Genre", UnlockValue = "Entertainment", Description = "예능 장르 해금" },
                new() { RequiredSubscribers = 100, UnlockType = "Milestone", UnlockValue = "Bronze", Description = "브론즈 크리에이터" },
                new() { RequiredSubscribers = 1000, UnlockType = "Milestone", UnlockValue = "Silver", Description = "실버 버튼" },
                new() { RequiredSubscribers = 10000, UnlockType = "Milestone", UnlockValue = "Gold", Description = "골드 버튼" },
                new() { RequiredSubscribers = 100000, UnlockType = "Milestone", UnlockValue = "Diamond", Description = "다이아 버튼" },
                new() { RequiredSubscribers = 1000000, UnlockType = "Milestone", UnlockValue = "Ruby", Description = "루비 버튼" },
                new() { RequiredSubscribers = 10000000, UnlockType = "Milestone", UnlockValue = "Ending", Description = "전설의 크리에이터 (엔딩)" },
            };
        }
    }
}
