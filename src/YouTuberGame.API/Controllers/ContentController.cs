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
    public class ContentController : ControllerBase
    {
        private readonly ContentService _contentService;
        private readonly ILogger<ContentController> _logger;

        public ContentController(ContentService contentService, ILogger<ContentController> logger)
        {
            _contentService = contentService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// 콘텐츠 제작 시작
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> StartContent([FromBody] StartContentRequest request)
        {
            var userId = GetUserId();
            _logger.LogInformation("Content start: {UserId} - {Title}", userId, request.Title);

            var response = await _contentService.StartContentAsync(userId, request);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        /// <summary>
        /// 제작 중인 콘텐츠 조회
        /// </summary>
        [HttpGet("producing")]
        public async Task<IActionResult> GetProducing()
        {
            var userId = GetUserId();
            var contents = await _contentService.GetProducingContentAsync(userId);
            return Ok(contents);
        }

        /// <summary>
        /// 제작 완료 처리
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteContent(string id)
        {
            var userId = GetUserId();
            _logger.LogInformation("Content complete: {UserId} - {ContentId}", userId, id);

            var response = await _contentService.CompleteContentAsync(userId, id);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        /// <summary>
        /// 콘텐츠 업로드
        /// </summary>
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadContent(string id)
        {
            var userId = GetUserId();
            _logger.LogInformation("Content upload: {UserId} - {ContentId}", userId, id);

            var response = await _contentService.UploadContentAsync(userId, id);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        /// <summary>
        /// 업로드 히스토리
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetUserId();
            var contents = await _contentService.GetContentHistoryAsync(userId, page, pageSize);
            return Ok(contents);
        }
    }
}
