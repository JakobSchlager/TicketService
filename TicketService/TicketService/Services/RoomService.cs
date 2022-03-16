using System.Net.Http;
using System.Threading.Tasks;
using TicketService.DTOs; 

namespace TicketService.Services
{
    public class RoomService
    {
        private static HttpClient client = new HttpClient(); 
        public async Task<SeatDto> GetSeat(int id)
        {
            SeatDto seat = null; 
            HttpResponseMessage response = await client.GetAsync($"http://roomservice/api/seats/{id}");
            if (response.IsSuccessStatusCode) seat = await response.Content.ReadAsAsync<SeatDto>();
            return seat;
        }
    }
}
