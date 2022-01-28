using System.Net.Http;
using System.Threading.Tasks;
using TicketService.DTOs;

namespace TicketService.Services
{
    public class MovieService
    {
        private static HttpClient httpClient = new HttpClient(); 
        public async Task<PresentationDto> GetPresentation(int id)
        {
            PresentationDto presentation = null; 
            HttpResponseMessage response = await httpClient.GetAsync($"http://movieservice/api/presentations/{id}");
            if (response.IsSuccessStatusCode)
            {
                presentation = await response.Content.ReadAsAsync<PresentationDto>();
            }

            return presentation;
        }

        public async Task<MovieDto> GetMovie(int id)
        {
            MovieDto movie = null; 
            HttpResponseMessage response = await httpClient.GetAsync($"http://movieservice/api/movies/{id}");
            if (response.IsSuccessStatusCode) movie = await response.Content.ReadAsAsync<MovieDto>();
            return movie;
        }
    }
}

