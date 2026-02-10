using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTuberGame.API.Services;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GachaController : ControllerBase
    {
        private readonly GachaService _gachaService;
        private readonly ILogger<GachaController> _logger;

        public GachaController(GachaService gachaService, ILogger<GachaController> logger)
        {
            _gachaService = gachaService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        [HttpPost("draw")]
        public async Task<IActionResult> DrawGacha([FromBody] GachaRequest request)
        {
            var userId = GetUserId();
            _logger.LogInformation("Gacha draw: {UserId} - Count: {Count}, UseTicket: {UseTicket}",
                userId, request.DrawCount, request.UseTicket);

            // 검증
            if (request.DrawCount < 1 || request.DrawCount > 10)
            {
                return BadRequest(new { Message = "1~10회까지 뽑을 수 있습니다." });
            }

            var response = await _gachaService.DrawGachaAsync(userId, request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
