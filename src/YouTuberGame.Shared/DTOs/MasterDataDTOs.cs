namespace YouTuberGame.Shared.DTOs
{
    /// <summary>
    /// 마스터 데이터 전체 응답
    /// 클라이언트가 앱 시작 시 한 번 받아서 캐싱
    /// </summary>
    public class MasterDataResponse
    {
        public int Version { get; set; }
        public GachaBalanceData Gacha { get; set; } = new();
        public CharacterBalanceData Character { get; set; } = new();
        public ContentBalanceData Content { get; set; } = new();
        public EquipmentBalanceData Equipment { get; set; } = new();
        public PlayerStartData PlayerStart { get; set; } = new();
        public List<MilestoneData> Milestones { get; set; } = new();
    }

    /// <summary>
    /// 버전만 확인하는 응답
    /// </summary>
    public class MasterDataVersionResponse
    {
        public int Version { get; set; }
    }

    /// <summary>
    /// Key-Value 엔트리 (Unity JsonUtility가 Dictionary를 못 파싱하므로 배열로 전송)
    /// </summary>
    public class KeyValueEntry
    {
        public string Key { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    // ───── 가챠 밸런스 ─────
    public class GachaBalanceData
    {
        public int GemCostPerDraw { get; set; }
        public int MaxDrawCount { get; set; }
        public List<KeyValueEntry> RarityProbability { get; set; } = new();
        // S=5, A=15, B=30, C=50 (합계 100)
    }

    // ───── 캐릭터 밸런스 ─────
    public class CharacterBalanceData
    {
        public List<KeyValueEntry> MaxLevelByRarity { get; set; } = new();
        // C=30, B=40, A=50, S=60
        public int BreakthroughLevelBonus { get; set; }
        // 돌파당 추가 레벨 (+10)
        public int ExpPerChip { get; set; }
        // 경험치 칩 1개 = 100 EXP
        public float LevelStatMultiplier { get; set; }
        // 레벨당 스탯 증가율 (0.02 = 2%)
    }

    // ───── 콘텐츠 밸런스 ─────
    public class ContentBalanceData
    {
        public List<KeyValueEntry> ProductionTimeByGenre { get; set; } = new();
        // Vlog=300, Gaming=600 ...
        public int ViewsPerQualityMin { get; set; }
        public int ViewsPerQualityMax { get; set; }
        // 조회수 = 품질 * random(min~max)
        public int LikesPercentMin { get; set; }
        public int LikesPercentMax { get; set; }
        // 좋아요 = 조회수 * random(min~max) / 100
        public int ViewsPerGold { get; set; }
        // 수익 = 조회수 / viewsPerGold
        public int SubscriberPerViewsMin { get; set; }
        public int SubscriberPerViewsMax { get; set; }
        // 구독자 = 조회수 / random(min~max)
    }

    // ───── 장비 밸런스 ─────
    public class EquipmentBalanceData
    {
        public int MaxLevel { get; set; }
        public int CostPerLevel { get; set; }
        // 업그레이드 비용 = level * costPerLevel
        public int BonusPerLevel { get; set; }
        // 보너스 = level * bonusPerLevel
    }

    // ───── 신규 유저 시작 데이터 ─────
    public class PlayerStartData
    {
        public long StartGold { get; set; }
        public int StartGems { get; set; }
        public int StartTickets { get; set; }
        public int StartExpChips { get; set; }
    }

    // ───── 마일스톤 (장르 해금 등) ─────
    public class MilestoneData
    {
        public long RequiredSubscribers { get; set; }
        public string UnlockType { get; set; } = string.Empty;
        // Genre, Feature 등
        public string UnlockValue { get; set; } = string.Empty;
        // Gaming, Mukbang 등
        public string Description { get; set; } = string.Empty;
    }
}
