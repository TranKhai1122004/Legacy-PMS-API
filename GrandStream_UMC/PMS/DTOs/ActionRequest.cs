using System.ComponentModel.DataAnnotations;

namespace PMS.DTOs
{
    public class ActionRequest
    {
        [Required(ErrorMessage = "Hành động (Action) không được để trống")]
        public string Action { get; set; } = "";

        [Required(ErrorMessage = "Mã phòng (Room) không được để trống")]
        public string Room { get; set; } = "";

        [Required(ErrorMessage = "Tên không được để trống")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Họ không được để trống")]
        public string LastName { get; set; } = "";
        public string NewRoom { get; set; } = "";
        public string WakeUpTime { get; set; } = "";
    }
}