using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTuberGame.API.Services;

namespace YouTuberGame.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/rankings")]
    public class RankingController : ControllerBase
    {
        private readonly RankingService _rankingService;
        private readonly ILogger<RankingController> _logger;

        public RankingController(RankingService rankingService, ILogger<RankingController> logger)
        {
            _rankingService = rankingService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// 주간 랭킹 (구독자 수 기준)
        /// </summary>
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyRanking([FromQuery] int topCount = 100)
        {
            var userId = GetUserId();
            var ranking = await _rankingService.GetWeeklyRankingAsync(userId, topCount);
            return Ok(ranking);
        }

        /// <summary>
        /// 채널 파워 랭킹
        /// </summary>
        [HttpGet("channel-power")]
        public async Task<IActionResult> GetChannelPowerRanking([FromQuery] int topCount = 100)
        {
            var userId = GetUserId();
            var ranking = await _rankingService.GetChannelPowerRankingAsync(userId, topCount);
            return Ok(ranking);
        }
    }
}
