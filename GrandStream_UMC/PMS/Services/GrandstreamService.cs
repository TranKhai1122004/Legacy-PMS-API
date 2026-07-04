using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PMS.DTOs; 

namespace PMS.Services
{
    
    public class GrandstreamService : IGrandstreamService
    {
        private readonly HttpClient _httpClient;
        private readonly string _mockoonUrl = "http://localhost:8089";

        public GrandstreamService()
        {
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (m, c, ch, e) => true };
            _httpClient = new HttpClient(handler);
        }

        
        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_mockoonUrl}/api/login?username={username}&password={password}",
                    null
                );

                Console.WriteLine($"Status: {response.StatusCode}");

                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);

                if (response.IsSuccessStatusCode)
                {
                    return body;
                }

                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        
        public async Task<object> SendPmsActionAsync(ActionRequest request)
        {
            try
            {
                
                var queryParams = new List<string>
        {
            $"action={request.Action}",
            $"address={request.Room}"
        };

                
                switch (request.Action?.ToLower())
                {
                    case "checkin":
                        queryParams.Add($"action=checkin");
                        queryParams.Add($"firstname={Uri.EscapeDataString(request.FirstName ?? "")}");
                        queryParams.Add($"lastname={Uri.EscapeDataString(request.LastName ?? "")}");
                        break;

                    case "checkout":
                        queryParams.Add($"action=checkout");

                        break;

                    case "roommove":
                        queryParams.Add($"action=roommove");


                        break;

                    case "wakeup":
                        queryParams.Add($"action=wakeup");

                        break;

                    default:
                        
                        queryParams.Add($"action={Uri.EscapeDataString(request.Action ?? "")}");
                        break;
                }

                
                string queryString = string.Join("&", queryParams);
                string url = $"{_mockoonUrl}/api/pms_action?{queryString}";

                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return new { success = false, message = "Không kết nối được API PMS" };

                string jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                int resultCode = doc.RootElement.GetProperty("result").GetInt32();

                
                string mockoonMessage = doc.RootElement.TryGetProperty("message", out var msgProp)
                                        ? (msgProp.GetString() ?? "Thao tác thành công")
                                        : "Thao tác thành công";

                return new
                {
                    success = (resultCode == 0),
                    message = mockoonMessage
                };
            }
            catch
            {
                return new { success = false, message = "Hệ thống gặp sự cố xử lý" };
            }
        }

        
        public async Task<RoomBillResponse?> GetRoomBillAndLogsAsync(string roomNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_mockoonUrl}/api/cdrapi");
                if (!response.IsSuccessStatusCode) return null; // Trả về null nếu lỗi kết nối

                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                decimal totalCost = 0;
                
                var roomCalls = new List<CallLogDto>();


                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.GetProperty("caller").GetString() == roomNumber)
                    {
                        string? callee = item.GetProperty("callee").GetString();
                        int duration = item.GetProperty("duration").GetInt32();
                        string? status = item.GetProperty("status").GetString();

                        
                        if ((callee?.Length ?? 0) > 3)
                        {
                            totalCost += Math.Ceiling((decimal)duration / 60) * 1000;
                        }

                       
                        roomCalls.Add(new CallLogDto
                        {
                            Callee = callee,
                            Duration = duration,
                            Status = status
                        });
                    }
                }

                
                return new RoomBillResponse
                {
                    Room = roomNumber,
                    TotalCost = totalCost,
                    Calls = roomCalls
                };
            }
            catch { return null; }
        }
    }
}