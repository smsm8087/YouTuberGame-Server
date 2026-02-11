using Microsoft.EntityFrameworkCore;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Services
{
    public class EquipmentService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(GameDbContext context, ILogger<EquipmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 업그레이드 비용: Level * 500 Gold
        private static int GetUpgradeCost(int currentLevel) => currentLevel * 500;

        // 장비 레벨당 보너스 (콘텐츠 제작 시 적용)
        private static int GetBonusValue(int level) => level * 5;

        public async Task<List<EquipmentResponse>> GetPlayerEquipmentAsync(string userId)
        {
            try
            {
                // 플레이어의 모든 장비 조회
                var equipment = await _context.PlayerEquipment
                    .Where(e => e.UserId == userId)
                    .ToListAsync();

                // 없으면 초기화 (신규 유저)
                if (equipment.Count == 0)
                {
                    await InitializeEquipmentAsync(userId);
                    equipment = await _context.PlayerEquipment
                        .Where(e => e.UserId == userId)
                        .ToListAsync();
                }

                var response = equipment.Select(e => new EquipmentResponse
                {
                    EquipmentType = e.EquipmentType,
                    Level = e.Level,
                    UpgradeCost = GetUpgradeCost(e.Level),
                    BonusValue = GetBonusValue(e.Level)
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player equipment");
                return new List<EquipmentResponse>();
            }
        }

        public async Task<UpgradeEquipmentResponse> UpgradeEquipmentAsync(string userId, EquipmentType equipmentType)
        {
            try
            {
                var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);
                if (playerData == null)
                {
                    return new UpgradeEquipmentResponse { Success = false, Message = "플레이어 데이터를 찾을 수 없습니다." };
                }

                var equipment = await _context.PlayerEquipment
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.EquipmentType == equipmentType);

                if (equipment == null)
                {
                    // 장비가 없으면 초기화
                    await InitializeEquipmentAsync(userId);
                    equipment = await _context.PlayerEquipment
                        .FirstOrDefaultAsync(e => e.UserId == userId && e.EquipmentType == equipmentType);
                }

                if (equipment == null)
                {
                    return new UpgradeEquipmentResponse { Success = false, Message = "장비를 찾을 수 없습니다." };
                }

                int upgradeCost = GetUpgradeCost(equipment.Level);

                if (playerData.Gold < upgradeCost)
                {
                    return new UpgradeEquipmentResponse
                    {
                        Success = false,
                        Message = $"골드가 부족합니다. (필요: {upgradeCost:N0})"
                    };
                }

                // 업그레이드 실행
                playerData.Gold -= upgradeCost;
                equipment.Level++;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Equipment upgraded: {UserId} - {Type} to Lv.{Level}",
                    userId, equipmentType, equipment.Level);

                return new UpgradeEquipmentResponse
                {
                    Success = true,
                    Message = $"{GetEquipmentName(equipmentType)} 업그레이드 성공!",
                    NewLevel = equipment.Level,
                    NextUpgradeCost = GetUpgradeCost(equipment.Level),
                    RemainingGold = playerData.Gold
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading equipment");
                return new UpgradeEquipmentResponse { Success = false, Message = "업그레이드 중 오류가 발생했습니다." };
            }
        }

        private async Task InitializeEquipmentAsync(string userId)
        {
            // 4종류 장비 초기화 (레벨 1)
            var equipment = new List<PlayerEquipment>
            {
                new PlayerEquipment { UserId = userId, EquipmentType = EquipmentType.Camera, Level = 1 },
                new PlayerEquipment { UserId = userId, EquipmentType = EquipmentType.Microphone, Level = 1 },
                new PlayerEquipment { UserId = userId, EquipmentType = EquipmentType.Light, Level = 1 },
                new PlayerEquipment { UserId = userId, EquipmentType = EquipmentType.PC, Level = 1 }
            };

            _context.PlayerEquipment.AddRange(equipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Equipment initialized for user: {UserId}", userId);
        }

        private static string GetEquipmentName(EquipmentType type) => type switch
        {
            EquipmentType.Camera => "카메라",
            EquipmentType.Microphone => "마이크",
            EquipmentType.Light => "조명",
            EquipmentType.PC => "컴퓨터",
            _ => "장비"
        };
    }
}
