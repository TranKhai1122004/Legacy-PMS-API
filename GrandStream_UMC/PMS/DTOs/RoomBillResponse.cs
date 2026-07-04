using System.Collections.Generic;

namespace PMS.DTOs
{
    public class CallLogDto
    {
        public string? Callee { get; set; }
        public int Duration { get; set; }
        public string? Status { get; set; }
    }

    public class RoomBillResponse
    {
        public string Room { get; set; } = "";
        public decimal TotalCost { get; set; }
        public List<CallLogDto> Calls { get; set; } = new List<CallLogDto>();
    }
}