using System.Threading.Tasks;
using PMS.DTOs; 

namespace PMS.Services
{
   
    public interface IGrandstreamService
    {
        Task<string> LoginAsync(string username, string password);

        Task<object> SendPmsActionAsync(ActionRequest request);

        Task<RoomBillResponse?> GetRoomBillAndLogsAsync(string roomNumber);
    }
}