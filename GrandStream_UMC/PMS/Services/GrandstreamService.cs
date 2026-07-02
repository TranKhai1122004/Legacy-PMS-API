using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PMS_Real.Services
{
    // 1. INTERFACE QUẢN LÝ 3 HÀM (Login, Điều khiển chung, và Hàm gộp dữ liệu cuộc gọi + Bill)
    public interface IGrandstreamService
    {
        Task<string> LoginAsync(string username, string password);
        Task<bool> SendPmsActionAsync(string action, string room, string firstName, string lastName, string newRoom, string wakeUpTime = null);
        Task<object> GetRoomBillAndLogsAsync(string roomNumber); // Hàm gộp đúng yêu cầu của bạn
    }

    // 2. CLASS TRIỂN KHAI LOGIC CHI TIẾT
    public class GrandstreamService : IGrandstreamService
    {
        private readonly HttpClient _httpClient;
        private readonly string _mockoonUrl = "http://localhost:8089";

        public GrandstreamService()
        {
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (m, c, ch, e) => true };
            _httpClient = new HttpClient(handler);
        }

        // --- Đăng nhập ---
        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_mockoonUrl}/api/login?username={username}&password={password}");
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
            catch { return null; }
        }

        // --- Hàm xử lý chung 3 nghiệp vụ: Check-in/Check-out, Room Move, Wake-up Call ---
        public async Task<bool> SendPmsActionAsync(string action, string room, string firstName, string lastName, string newRoom, string wakeUpTime = null)
        {
            try
            {
                string targetRoom = action == "roommove" ? newRoom : room;
                string timeParam = action == "wakeup" ? $"&time={wakeUpTime ?? ""}" : "";

                string url = $"{_mockoonUrl}/api/pms_action?action={action}&address={room}&room={targetRoom}&firstname={firstName ?? ""}&lastname={lastName ?? ""}{timeParam}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return false;
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return doc.RootElement.GetProperty("result").GetInt32() == 0;
            }
            catch { return false; }
        }

        // --- HÀM GỘP: Vừa lọc lịch sử cuộc gọi gốc, vừa tính tiền hóa đơn ---
        public async Task<object> GetRoomBillAndLogsAsync(string roomNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_mockoonUrl}/api/cdrapi");
                if (!response.IsSuccessStatusCode) return null;
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                decimal totalCost = 0;
                var roomCalls = new List<object>();

                // Lọc mảng dữ liệu trả về từ Mockoon/Tổng đài
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.GetProperty("caller").GetString() == roomNumber)
                    {
                        string callee = item.GetProperty("callee").GetString();
                        int duration = item.GetProperty("duration").GetInt32();
                        string status = item.GetProperty("status").GetString();

                        // 1. Tính tổng tiền hóa đơn ngầm dựa trên danh sách cuộc gọi ngoại mạng
                        if (callee.Length > 3)
                        {
                            totalCost += Math.Ceiling((decimal)duration / 60) * 1000;
                        }

                        // 2. Thêm vào mảng lịch sử cuộc gọi gốc (chỉ lưu thông tin thô)
                        roomCalls.Add(new
                        {
                            callee,
                            duration,
                            status
                        });
                    }
                }

                // Trả ra một cục data duy nhất chứa cả 2 thông tin
                return new
                {
                    room = roomNumber,
                    totalCost,
                    calls = roomCalls
                };
            }
            catch { return null; }
        }
    }
}