using Microsoft.AspNetCore.Identity;
using PaginaAnimeListMVC.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Show> LikedShows { get; set; } = new List<Show>();
    public ICollection<Show> WatchingShows { get; set; } = new List<Show>();
    public ICollection<Show> WatchlistShows { get; set; } = new List<Show>();
}
