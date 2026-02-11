using Microsoft.AspNetCore.Mvc;
using YouTuberGame.API.Services;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;

        public AdminController(AdminService adminService, ILogger<AdminController> logger, IConfiguration configuration)
        {
            _adminService = adminService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// 어드민 인증 체크 (간단한 비밀번호 방식)
        /// </summary>
        private bool CheckAdminAuth()
        {
            var adminPassword = _configuration["Admin:Password"];
            var authHeader = Request.Headers["X-Admin-Password"].FirstOrDefault();
            return authHeader == adminPassword;
        }

        /// <summary>
        /// 어드민 로그인 체크
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginRequest request)
        {
            var adminPassword = _configuration["Admin:Password"];
            if (request.Password == adminPassword)
            {
                return Ok(new { Success = true, Message = "로그인 성공" });
            }
            return Unauthorized(new { Success = false, Message = "잘못된 비밀번호" });
        }

        /// <summary>
        /// 대시보드 데이터
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var data = await _adminService.GetDashboardDataAsync();
            return Ok(data);
        }

        /// <summary>
        /// 유저 목록 조회
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var result = await _adminService.GetUsersAsync(search, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// 유저 상세 조회
        /// </summary>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetail(string userId)
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var result = await _adminService.GetUserDetailAsync(userId);
            if (result == null)
                return NotFound(new { Success = false, Message = "유저를 찾을 수 없습니다" });

            return Ok(result);
        }

        /// <summary>
        /// 유저 재화 수정
        /// </summary>
        [HttpPut("users/{userId}/currency")]
        public async Task<IActionResult> UpdateCurrency(string userId, [FromBody] UpdateCurrencyRequest request)
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var success = await _adminService.UpdateUserCurrencyAsync(userId, request);
            if (!success)
                return NotFound(new { Success = false, Message = "유저를 찾을 수 없습니다" });

            return Ok(new { Success = true, Message = "재화가 수정되었습니다" });
        }

        /// <summary>
        /// 보상 지급
        /// </summary>
        [HttpPost("rewards/send")]
        public async Task<IActionResult> SendReward([FromBody] SendRewardRequest request)
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var result = await _adminService.SendRewardAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// 가챠 통계
        /// </summary>
        [HttpGet("statistics/gacha")]
        public async Task<IActionResult> GetGachaStatistics()
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var stats = await _adminService.GetGachaStatisticsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// 콘텐츠 통계
        /// </summary>
        [HttpGet("statistics/content")]
        public async Task<IActionResult> GetContentStatistics()
        {
            if (!CheckAdminAuth())
                return Unauthorized(new { Success = false, Message = "권한 없음" });

            var stats = await _adminService.GetContentStatisticsAsync();
            return Ok(stats);
        }
    }

    public class AdminLoginRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}
