using Microsoft.AspNetCore.Mvc;
using PMS_Real.Services;
using System.Threading.Tasks;

namespace PMS_Real.Controllers
{
    [ApiController]
    [Route("api/")]
    // Cấu hình CORS để ReactJS (thường chạy ở port 3000) có thể gọi vào .NET
    [Microsoft.AspNetCore.Cors.EnableCors("AllowReact")]
    public class PmsController : ControllerBase
    {
        private readonly IGrandstreamService _grandstreamService;

        public PmsController(IGrandstreamService grandstreamService)
        {
            _grandstreamService = grandstreamService;
        }

        // API Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Kiểm tra dữ liệu đầu vào trống
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { success = false, message = "Tài khoản và mật khẩu không được để trống" });
            }

            // 2. Gọi Service để lấy token/kết quả từ Mockoon
            var token = await _grandstreamService.LoginAsync(request.Username, request.Password);

            // 3. Nếu token hợp lệ (không null hoặc rỗng)
            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { success = true, message = "Đăng nhập thành công", token });
            }

            // 4. Nếu thất bại, trả về 401 Unauthorized (đúng chuẩn bảo mật hơn 400)
            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu" });
        }

        // API Gộp: Lấy lịch sử cuộc gọi và tổng tiền hóa đơn
        [HttpGet("bill")]
        public async Task<IActionResult> GetBill([FromQuery] string room)
        {
            var data = await _grandstreamService.GetRoomBillAndLogsAsync(room);
            return data != null ? Ok(data) : NotFound(new { message = "Không tìm thấy phòng" });
        }

        // API Điều khiển: Check-in, Checkout, RoomMove, Wakeup
        [HttpPost("action")]
        public async Task<IActionResult> HandleAction([FromBody] ActionRequest request)
        {
            bool isSuccess = await _grandstreamService.SendPmsActionAsync(
                request.Action, request.Room, request.FirstName, request.LastName, request.NewRoom, request.WakeUpTime
            );
            return Ok(new { success = isSuccess, message = isSuccess ? "Thao tác thành công" : "Thao tác thất bại" });
        }
    }

    public class LoginRequest { public string Username { get; set; } public string Password { get; set; } }
    public class ActionRequest
    {
        public string Action { get; set; }
        public string Room { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NewRoom { get; set; }
        public string WakeUpTime { get; set; }
    }
}