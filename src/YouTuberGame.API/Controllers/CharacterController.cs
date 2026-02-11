using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YouTuberGame.API.Data;
using YouTuberGame.API.Services;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class CharacterController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly CharacterService _characterService;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(GameDbContext context, CharacterService characterService, ILogger<CharacterController> logger)
        {
            _context = context;
            _characterService = characterService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// 전체 캐릭터 목록 (도감)
        /// </summary>
        [HttpGet("characters")]
        public async Task<IActionResult> GetAllCharacters()
        {
            var characters = await _context.Characters
                .Select(c => new CharacterResponse
                {
                    CharacterId = c.CharacterId,
                    CharacterName = c.CharacterName,
                    Rarity = c.Rarity,
                    Specialty = c.Specialty,
                    BaseFilming = c.BaseFilming,
                    BaseEditing = c.BaseEditing,
                    BasePlanning = c.BasePlanning,
                    BaseDesign = c.BaseDesign,
                    PassiveSkillDesc = c.PassiveSkillDesc,
                    PassiveSkillValue = c.PassiveSkillValue
                })
                .ToListAsync();

            return Ok(characters);
        }

        /// <summary>
        /// 캐릭터 레벨업 (ExpChips 소모)
        /// </summary>
        [Authorize]
        [HttpPost("player/characters/{instanceId}/levelup")]
        public async Task<IActionResult> LevelUp(string instanceId, [FromBody] LevelUpRequest request)
        {
            var userId = GetUserId();
            _logger.LogInformation("Character levelup: {UserId} - {InstanceId}, ExpChips: {ExpChips}",
                userId, instanceId, request.ExpChipsToUse);

            var response = await _characterService.LevelUpAsync(userId, instanceId, request);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        /// <summary>
        /// 캐릭터 돌파 (같은 캐릭터 카드 소모)
        /// </summary>
        [Authorize]
        [HttpPost("player/characters/{instanceId}/breakthrough")]
        public async Task<IActionResult> Breakthrough(string instanceId, [FromBody] BreakthroughRequest request)
        {
            var userId = GetUserId();
            _logger.LogInformation("Character breakthrough: {UserId} - {InstanceId}", userId, instanceId);

            var response = await _characterService.BreakthroughAsync(userId, instanceId, request);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
