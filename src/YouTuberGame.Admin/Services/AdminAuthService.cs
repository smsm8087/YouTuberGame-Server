namespace YouTuberGame.Admin.Services
{
    public class AdminAuthService
    {
        private bool _isAuthenticated = false;
        private string? _adminPassword;

        public AdminAuthService(IConfiguration configuration)
        {
            _adminPassword = configuration["Admin:Password"];
        }

        public bool IsAuthenticated => _isAuthenticated;

        public bool Login(string password)
        {
            if (password == _adminPassword)
            {
                _isAuthenticated = true;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _isAuthenticated = false;
        }
    }
}
