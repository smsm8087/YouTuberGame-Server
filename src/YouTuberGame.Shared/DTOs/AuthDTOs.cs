using System.ComponentModel.DataAnnotations;

namespace YouTuberGame.Shared.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ChannelName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? PlayerName { get; set; }
        public string? Message { get; set; }
    }
}
