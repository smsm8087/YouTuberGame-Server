using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YouTuberGame.Shared.DTOs;

namespace YouTuberGame.Admin.Services
{
    public class AdminApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _adminPassword;

        public AdminApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _adminPassword = configuration["Admin:Password"] ?? "";

            var apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";
            _httpClient.BaseAddress = new Uri(apiUrl);
        }

        private void SetAdminHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Admin-Password");
            _httpClient.DefaultRequestHeaders.Add("X-Admin-Password", _adminPassword);
        }

        public async Task<DashboardData?> GetDashboardAsync()
        {
            SetAdminHeader();
            var response = await _httpClient.GetAsync("/api/admin/dashboard");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DashboardData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<AdminUserListResponse?> GetUsersAsync(string? search = null, int page = 1)
        {
            SetAdminHeader();
            var url = $"/api/admin/users?page={page}&pageSize=50";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AdminUserListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<AdminUserDetailResponse?> GetUserDetailAsync(string userId)
        {
            SetAdminHeader();
            var response = await _httpClient.GetAsync($"/api/admin/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AdminUserDetailResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<bool> UpdateUserCurrencyAsync(string userId, UpdateCurrencyRequest request)
        {
            SetAdminHeader();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/admin/users/{userId}/currency", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<SendRewardResponse?> SendRewardAsync(SendRewardRequest request)
        {
            SetAdminHeader();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/admin/rewards/send", content);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SendRewardResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<GachaStatistics?> GetGachaStatisticsAsync()
        {
            SetAdminHeader();
            var response = await _httpClient.GetAsync("/api/admin/statistics/gacha");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GachaStatistics>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<ContentStatistics?> GetContentStatisticsAsync()
        {
            SetAdminHeader();
            var response = await _httpClient.GetAsync("/api/admin/statistics/content");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ContentStatistics>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
    }
}
