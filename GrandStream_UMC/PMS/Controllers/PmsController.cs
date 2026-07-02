using Microsoft.AspNetCore.Mvc;
using PMS_Real.Services;
using System.Threading.Tasks;

namespace PMS_Real.Controllers
{
    [ApiController]
    [Route("api")]
    public class PmsController : ControllerBase
    {
        private readonly IGrandstreamService _grandstreamService;

        // Tiêm Service qua Constructor Injection giống hệt NestJS
        public PmsController(IGrandstreamService grandstreamService)
        {
            _grandstreamService = grandstreamService;
        }

        [HttpGet("back-login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            // Truyền tiếp 2 tham số này vào Service
            var result = await _grandstreamService.LoginAsync(username, password);
            return result != null ? Content(result, "application/json") : BadRequest(new { message = "Lỗi kết nối hoặc sai tài khoản/mật khẩu" });
        }

        [HttpGet("back-action")]
        public async Task<IActionResult> HandleAction(string action, string room, string firstName = null, string lastName = null, string newRoom = null)
        {
            bool isSuccess = await _grandstreamService.SendPmsActionAsync(action, room, firstName, lastName, newRoom);
            return Ok(new { success = isSuccess, action, room });
        }

        [HttpGet("back-bill")]
        public async Task<IActionResult> GetBill([FromQuery] string room)
        {
            // Gọi thẳng hàm gộp để lấy cục dữ liệu vừa có tiền vừa có lịch sử chi tiết
            var billAndLogsData = await _grandstreamService.GetRoomBillAndLogsAsync(room);

            return billAndLogsData != null
                ? Ok(billAndLogsData)
                : NotFound(new { message = "Không tìm thấy dữ liệu phòng" });
        }
    }
}