using Microsoft.EntityFrameworkCore;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Services
{
    public class GachaService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GachaService> _logger;
        private readonly Random _random = new Random();

        public GachaService(GameDbContext context, ILogger<GachaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GachaResponse> DrawGachaAsync(string userId, GachaRequest request)
        {
            try
            {
                var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);
                if (playerData == null)
                {
                    return new GachaResponse
                    {
                        Success = false,
                        Message = "플레이어 데이터를 찾을 수 없습니다."
                    };
                }

                // 비용 체크
                if (request.UseTicket)
                {
                    if (playerData.GachaTickets < request.DrawCount)
                    {
                        return new GachaResponse
                        {
                            Success = false,
                            Message = "가챠 티켓이 부족합니다."
                        };
                    }
                    playerData.GachaTickets -= request.DrawCount;
                }
                else
                {
                    int gemCost = request.DrawCount * 100; // 1회 100보석
                    if (playerData.Gems < gemCost)
                    {
                        return new GachaResponse
                        {
                            Success = false,
                            Message = "보석이 부족합니다."
                        };
                    }
                    playerData.Gems -= gemCost;
                }

                // 가챠 실행
                var results = new List<GachaResult>();
                var allCharacters = await _context.Characters.ToListAsync();
                var ownedCharacterIds = await _context.PlayerCharacters
                    .Where(pc => pc.UserId == userId)
                    .Select(pc => pc.CharacterId)
                    .ToListAsync();
                var ownedSet = new HashSet<string>(ownedCharacterIds);

                for (int i = 0; i < request.DrawCount; i++)
                {
                    var rarity = GetRandomRarity();
                    var character = GetRandomCharacterByRarity(allCharacters, rarity);

                    if (character != null)
                    {
                        bool isNew = !ownedSet.Contains(character.CharacterId);
                        ownedSet.Add(character.CharacterId);

                        // 캐릭터 인스턴스 생성
                        var playerCharacter = new PlayerCharacter
                        {
                            UserId = userId,
                            CharacterId = character.CharacterId
                        };

                        _context.PlayerCharacters.Add(playerCharacter);

                        results.Add(new GachaResult
                        {
                            InstanceId = playerCharacter.InstanceId,
                            CharacterId = character.CharacterId,
                            CharacterName = character.CharacterName,
                            Rarity = character.Rarity,
                            Specialty = character.Specialty,
                            IsNew = isNew
                        });

                        _logger.LogInformation("Gacha draw: {UserId} got {CharacterName} ({Rarity})",
                            userId, character.CharacterName, character.Rarity);
                    }
                }

                await _context.SaveChangesAsync();

                return new GachaResponse
                {
                    Success = true,
                    Results = results,
                    RemainingTickets = playerData.GachaTickets,
                    RemainingGems = playerData.Gems,
                    Message = $"{request.DrawCount}회 가챠 성공!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gacha draw");
                return new GachaResponse
                {
                    Success = false,
                    Message = "가챠 중 오류가 발생했습니다."
                };
            }
        }

        private CharacterRarity GetRandomRarity()
        {
            var roll = _random.NextDouble() * 100;

            // C: 50%, B: 30%, A: 15%, S: 5%
            if (roll < 5) return CharacterRarity.S;
            if (roll < 20) return CharacterRarity.A;
            if (roll < 50) return CharacterRarity.B;
            return CharacterRarity.C;
        }

        private Character? GetRandomCharacterByRarity(List<Character> allCharacters, CharacterRarity rarity)
        {
            var candidates = allCharacters.Where(c => c.Rarity == rarity).ToList();
            if (candidates.Count == 0) return null;

            return candidates[_random.Next(candidates.Count)];
        }
    }
}
