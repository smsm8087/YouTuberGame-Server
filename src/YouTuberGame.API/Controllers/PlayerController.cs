using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(GameDbContext context, ILogger<PlayerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetPlayerData()
        {
            var userId = GetUserId();
            _logger.LogInformation("Getting player data: {UserId}", userId);

            var user = await _context.Users
                .Include(u => u.PlayerData)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.PlayerData == null)
            {
                return NotFound(new { Message = "플레이어 데이터를 찾을 수 없습니다." });
            }

            var response = new PlayerDataResponse
            {
                UserId = user.UserId,
                PlayerName = user.PlayerName,
                ChannelName = user.ChannelName,
                Subscribers = user.PlayerData.Subscribers,
                TotalViews = user.PlayerData.TotalViews,
                ChannelPower = user.PlayerData.ChannelPower,
                Gold = user.PlayerData.Gold,
                Gems = user.PlayerData.Gems,
                GachaTickets = user.PlayerData.GachaTickets,
                ExpChips = user.PlayerData.ExpChips,
                StudioLevel = user.PlayerData.StudioLevel
            };

            return Ok(response);
        }

        [HttpPut("save")]
        public async Task<IActionResult> UpdatePlayerData([FromBody] UpdatePlayerDataRequest request)
        {
            var userId = GetUserId();
            _logger.LogInformation("Updating player data: {UserId}", userId);

            var playerData = await _context.PlayerData.FirstOrDefaultAsync(p => p.UserId == userId);

            if (playerData == null)
            {
                return NotFound(new { Message = "플레이어 데이터를 찾을 수 없습니다." });
            }

            // 업데이트 (null이 아닌 값만)
            if (request.Gold.HasValue) playerData.Gold = request.Gold.Value;
            if (request.Gems.HasValue) playerData.Gems = request.Gems.Value;
            if (request.Subscribers.HasValue) playerData.Subscribers = request.Subscribers.Value;
            if (request.StudioLevel.HasValue) playerData.StudioLevel = request.StudioLevel.Value;

            playerData.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Player data updated: {UserId}", userId);

            return Ok(new { Success = true, Message = "저장 완료!" });
        }

        [HttpGet("characters")]
        public async Task<IActionResult> GetPlayerCharacters()
        {
            var userId = GetUserId();
            _logger.LogInformation("Getting player characters: {UserId}", userId);

            var characters = await _context.PlayerCharacters
                .Include(pc => pc.Character)
                .Where(pc => pc.UserId == userId)
                .Select(pc => new
                {
                    pc.InstanceId,
                    pc.CharacterId,
                    CharacterName = pc.Character!.CharacterName,
                    Rarity = pc.Character.Rarity,
                    Specialty = pc.Character.Specialty,
                    pc.Level,
                    pc.Experience,
                    pc.Breakthrough,
                    BaseStats = new
                    {
                        pc.Character.BaseFilming,
                        pc.Character.BaseEditing,
                        pc.Character.BasePlanning,
                        pc.Character.BaseDesign
                    },
                    pc.AcquiredAt
                })
                .ToListAsync();

            return Ok(new { Characters = characters });
        }
    }
}
