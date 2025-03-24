using Microsoft.AspNetCore.Identity;
using PaginaAnimeListMVC.Models;

namespace PaginaAnimeListMVC.Models{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ShowLike> ShowLikes { get; set; } = new List<ShowLike>();
        public ICollection<ShowWatchlist> ShowWatchlists { get; set; } = new List<ShowWatchlist>();
    }
}
