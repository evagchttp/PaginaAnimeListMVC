using PaginaAnimeListMVC.Models;

namespace PaginaAnimeListMVC.Services.Interfaces
{
    public interface IJikanApiService
    {
        public Task<List<Show>> GetTopShows();
        public Task<Show> GetShowById(int id); 
        public Task<List<Show>> GetShowsByIds(IEnumerable<int> ids);
        public Task<List<Show>> GetShowsBySearch(string query, int page = 1, int limit = 10, string type = "tv", bool sfw = true, List<string> genres = null);
        public Task<List<Show>> GetRelatedShows(int id, int limit = 12);
    }
}
