using Microsoft.AspNetCore.Mvc;
using PMS.Services;
using PMS.DTOs;
using System.Threading.Tasks;

namespace PMS.Controllers
{
    [ApiController]
    [Route("api")] // có thể để "api" thay vì "api/", .NET sẽ tự hiểu nối với route con
    [Microsoft.AspNetCore.Cors.EnableCors("AllowReact")]
    public class PmsController : ControllerBase
    {
        private readonly IGrandstreamService _grandstreamService;

        public PmsController(IGrandstreamService grandstreamService)
        {
            _grandstreamService = grandstreamService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            var token = await _grandstreamService.LoginAsync(request.Username, request.Password);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { success = true, message = "Đăng nhập thành công", token });
            }


            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu" });
        }


        [HttpGet("bill")]
        public async Task<IActionResult> GetBill([FromQuery] string room)
        {
            if (string.IsNullOrEmpty(room))
            {
                return BadRequest(new { message = "Mã phòng không được để trống" });
            }

            var data = await _grandstreamService.GetRoomBillAndLogsAsync(room);
            return data != null ? Ok(data) : NotFound(new { message = "Không tìm thấy phòng" });
        }


        [HttpPost("action")]
        public async Task<IActionResult> HandleAction([FromBody] ActionRequest request)
        {

            var resultObj = await _grandstreamService.SendPmsActionAsync(request);


            return Ok(resultObj);
        }
    }
}