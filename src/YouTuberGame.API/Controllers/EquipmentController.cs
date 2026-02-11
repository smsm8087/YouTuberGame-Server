using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTuberGame.API.Services;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/player")]
    public class EquipmentController : ControllerBase
    {
        private readonly EquipmentService _equipmentService;
        private readonly ILogger<EquipmentController> _logger;

        public EquipmentController(EquipmentService equipmentService, ILogger<EquipmentController> logger)
        {
            _equipmentService = equipmentService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// 장비 정보 조회
        /// </summary>
        [HttpGet("equipment")]
        public async Task<IActionResult> GetEquipment()
        {
            var userId = GetUserId();
            var equipment = await _equipmentService.GetPlayerEquipmentAsync(userId);
            return Ok(equipment);
        }

        /// <summary>
        /// 장비 업그레이드
        /// </summary>
        [HttpPost("equipment/{type}/upgrade")]
        public async Task<IActionResult> UpgradeEquipment(EquipmentType type)
        {
            var userId = GetUserId();
            _logger.LogInformation("Equipment upgrade: {UserId} - {Type}", userId, type);

            var response = await _equipmentService.UpgradeEquipmentAsync(userId, type);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
