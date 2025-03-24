using PaginaAnimeListMVC.Models;
namespace PaginaAnimeListMVC.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Show> Watchlist { get; set; }
        public List<Show> LikedShows { get; set; }
    }
}
