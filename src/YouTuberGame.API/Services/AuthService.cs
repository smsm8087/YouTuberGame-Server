using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YouTuberGame.API.Data;
using YouTuberGame.Shared.DTOs;
using YouTuberGame.Shared.Models;
using BCrypt.Net;

namespace YouTuberGame.API.Services
{
    public class AuthService
    {
        private readonly GameDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(GameDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // 이메일 중복 체크
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "이미 존재하는 이메일입니다."
                    };
                }

                // 비밀번호 해싱
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // 사용자 생성
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PlayerName = request.PlayerName,
                    ChannelName = request.ChannelName
                };

                _context.Users.Add(user);

                // 플레이어 데이터 초기화
                var playerData = new PlayerData
                {
                    UserId = user.UserId
                };

                _context.PlayerData.Add(playerData);

                // 장비 4종 초기화 (Lv.1)
                foreach (EquipmentType type in Enum.GetValues<EquipmentType>())
                {
                    _context.PlayerEquipment.Add(new PlayerEquipment
                    {
                        UserId = user.UserId,
                        EquipmentType = type,
                        Level = 1
                    });
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {UserId} - {Email}", user.UserId, user.Email);

                // JWT 토큰 생성
                var token = GenerateJwtToken(user);

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    UserId = user.UserId,
                    PlayerName = user.PlayerName,
                    Message = "회원가입 성공!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return new AuthResponse
                {
                    Success = false,
                    Message = "회원가입 중 오류가 발생했습니다."
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Email}", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "이메일 또는 비밀번호가 올바르지 않습니다."
                    };
                }

                // 비밀번호 확인
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password - {Email}", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "이메일 또는 비밀번호가 올바르지 않습니다."
                    };
                }

                // 마지막 로그인 시간 업데이트
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User logged in: {UserId} - {Email}", user.UserId, user.Email);

                // JWT 토큰 생성
                var token = GenerateJwtToken(user);

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    UserId = user.UserId,
                    PlayerName = user.PlayerName,
                    Message = "로그인 성공!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new AuthResponse
                {
                    Success = false,
                    Message = "로그인 중 오류가 발생했습니다."
                };
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("PlayerName", user.PlayerName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiryDays = int.Parse(_configuration["Jwt:ExpiryInDays"]!);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
