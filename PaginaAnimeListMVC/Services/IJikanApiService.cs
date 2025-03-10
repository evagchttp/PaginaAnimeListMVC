using PaginaAnimeListMVC.Models;

namespace PaginaAnimeListMVC.Services.Interfaces
{
    public interface IJikanApiService
    {
        public Task<List<Show>> GetTopShows();
        public Task<Show> GetShowById(int id); 
    }
}
