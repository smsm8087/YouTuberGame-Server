using Microsoft.AspNetCore.Mvc;
using YouTuberGame.API.Services;

namespace YouTuberGame.API.Controllers
{
    /// <summary>
    /// 마스터 데이터 API
    /// 인증 불필요 (앱 시작 시 로그인 전에도 받아야 함)
    /// </summary>
    [ApiController]
    [Route("api/master-data")]
    public class MasterDataController : ControllerBase
    {
        private readonly MasterDataService _masterDataService;
        private readonly ILogger<MasterDataController> _logger;

        public MasterDataController(MasterDataService masterDataService, ILogger<MasterDataController> logger)
        {
            _masterDataService = masterDataService;
            _logger = logger;
        }

        /// <summary>
        /// 마스터 데이터 버전 확인
        /// 클라이언트 캐시 버전과 비교하여 업데이트 필요 여부 판단
        /// </summary>
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new { Version = _masterDataService.GetVersion() });
        }

        /// <summary>
        /// 마스터 데이터 전체 조회
        /// 앱 시작 시 또는 버전 변경 시 호출
        /// </summary>
        [HttpGet]
        public IActionResult GetMasterData()
        {
            var data = _masterDataService.GetMasterData();
            _logger.LogInformation("마스터 데이터 v{Version} 전송", data.Version);
            return Ok(data);
        }
    }
}
