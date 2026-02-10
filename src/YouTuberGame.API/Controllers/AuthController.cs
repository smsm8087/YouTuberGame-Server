using Microsoft.AspNetCore.Mvc;
using YouTuberGame.API.Services;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Register attempt: {Email}", request.Email);

            var response = await _authService.RegisterAsync(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt: {Email}", request.Email);

            var response = await _authService.LoginAsync(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return Unauthorized(response);
        }
    }
}
