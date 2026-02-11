using Microsoft.EntityFrameworkCore;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Services
{
    public class CharacterService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<CharacterService> _logger;

        public CharacterService(GameDbContext context, ILogger<CharacterService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 레어리티별 기본 레벨 상한
        private static int GetBaseMaxLevel(CharacterRarity rarity) => rarity switch
        {
            CharacterRarity.C => 30,
            CharacterRarity.B => 40,
            CharacterRarity.A => 50,
            CharacterRarity.S => 60,
            _ => 30
        };

        // 레벨당 필요 경험치
        private static int GetRequiredExp(int level) => level * 100;

        // 현재 최대 레벨 (돌파 포함)
        private static int GetMaxLevel(CharacterRarity rarity, int breakthrough)
            => GetBaseMaxLevel(rarity) + (breakthrough * 10);

        public async Task<LevelUpResponse> LevelUpAsync(string userId, string instanceId, LevelUpRequest request)
        {
            try
            {
                var playerCharacter = await _context.PlayerCharacters
                    .Include(pc => pc.Character)
                    .FirstOrDefaultAsync(pc => pc.InstanceId == instanceId && pc.UserId == userId);

                if (playerCharacter == null)
                {
                    return new LevelUpResponse { Success = false, Message = "캐릭터를 찾을 수 없습니다." };
                }

                var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);
                if (playerData == null)
                {
                    return new LevelUpResponse { Success = false, Message = "플레이어 데이터를 찾을 수 없습니다." };
                }

                if (request.ExpChipsToUse < 1)
                {
                    return new LevelUpResponse { Success = false, Message = "최소 1개의 경험칩이 필요합니다." };
                }

                if (playerData.ExpChips < request.ExpChipsToUse)
                {
                    return new LevelUpResponse { Success = false, Message = "경험칩이 부족합니다." };
                }

                int maxLevel = GetMaxLevel(playerCharacter.Character!.Rarity, playerCharacter.Breakthrough);
                if (playerCharacter.Level >= maxLevel)
                {
                    return new LevelUpResponse
                    {
                        Success = false,
                        Message = "최대 레벨에 도달했습니다. 돌파가 필요합니다.",
                        NewLevel = playerCharacter.Level,
                        MaxLevel = maxLevel
                    };
                }

                // ExpChips 소모 및 경험치 적용
                playerData.ExpChips -= request.ExpChipsToUse;
                int expGained = request.ExpChipsToUse * 100; // 1 ExpChip = 100 EXP
                playerCharacter.Experience += expGained;

                // 레벨업 처리
                while (playerCharacter.Level < maxLevel)
                {
                    int required = GetRequiredExp(playerCharacter.Level);
                    if (playerCharacter.Experience < required) break;

                    playerCharacter.Experience -= required;
                    playerCharacter.Level++;
                }

                // 최대 레벨 도달 시 잉여 경험치 제거
                if (playerCharacter.Level >= maxLevel)
                {
                    playerCharacter.Experience = 0;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Character levelup: {UserId} - {InstanceId} to Lv.{Level}",
                    userId, instanceId, playerCharacter.Level);

                return new LevelUpResponse
                {
                    Success = true,
                    Message = $"레벨업 성공! Lv.{playerCharacter.Level}",
                    NewLevel = playerCharacter.Level,
                    CurrentExp = playerCharacter.Experience,
                    RequiredExp = playerCharacter.Level < maxLevel ? GetRequiredExp(playerCharacter.Level) : 0,
                    MaxLevel = maxLevel,
                    RemainingExpChips = playerData.ExpChips
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during character levelup");
                return new LevelUpResponse { Success = false, Message = "레벨업 중 오류가 발생했습니다." };
            }
        }

        public async Task<BreakthroughResponse> BreakthroughAsync(string userId, string instanceId, BreakthroughRequest request)
        {
            try
            {
                var playerCharacter = await _context.PlayerCharacters
                    .Include(pc => pc.Character)
                    .FirstOrDefaultAsync(pc => pc.InstanceId == instanceId && pc.UserId == userId);

                if (playerCharacter == null)
                {
                    return new BreakthroughResponse { Success = false, Message = "캐릭터를 찾을 수 없습니다." };
                }

                // 희생할 캐릭터 확인 (같은 캐릭터ID여야 함)
                var sacrifice = await _context.PlayerCharacters
                    .FirstOrDefaultAsync(pc => pc.InstanceId == request.SacrificeInstanceId && pc.UserId == userId);

                if (sacrifice == null)
                {
                    return new BreakthroughResponse { Success = false, Message = "희생할 캐릭터를 찾을 수 없습니다." };
                }

                if (sacrifice.CharacterId != playerCharacter.CharacterId)
                {
                    return new BreakthroughResponse { Success = false, Message = "같은 캐릭터만 돌파 재료로 사용할 수 있습니다." };
                }

                if (sacrifice.InstanceId == instanceId)
                {
                    return new BreakthroughResponse { Success = false, Message = "자기 자신을 재료로 사용할 수 없습니다." };
                }

                // 돌파 실행
                playerCharacter.Breakthrough++;
                _context.PlayerCharacters.Remove(sacrifice);

                await _context.SaveChangesAsync();

                int newMaxLevel = GetMaxLevel(playerCharacter.Character!.Rarity, playerCharacter.Breakthrough);

                _logger.LogInformation("Character breakthrough: {UserId} - {InstanceId} to Breakthrough {Breakthrough}",
                    userId, instanceId, playerCharacter.Breakthrough);

                return new BreakthroughResponse
                {
                    Success = true,
                    Message = $"돌파 성공! 최대 레벨: {newMaxLevel}",
                    NewBreakthrough = playerCharacter.Breakthrough,
                    NewMaxLevel = newMaxLevel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during character breakthrough");
                return new BreakthroughResponse { Success = false, Message = "돌파 중 오류가 발생했습니다." };
            }
        }
    }
}
