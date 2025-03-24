using PaginaAnimeListMVC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PaginaAnimeListMVC.Data;
using PaginaAnimeListMVC.ViewModels;
using PaginaAnimeListMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PaginaAnimeListMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJikanApiService _jikanApiService;

        public UserController(ApplicationDbContext context, IJikanApiService jikanApiService)
        {
            _context = context;
            _jikanApiService = jikanApiService;
        }

        public async Task<IActionResult> Perfil(){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return NotFound();
            }
            var watchlistIds = await _context.ShowWatchlists.Where(sw => sw.ApplicationUserId == user.Id).ToListAsync();
            var likedShowsIds = await _context.ShowLikes.Where(sl => sl.ApplicationUserId == user.Id).ToListAsync();
            var watchlistShows = await _jikanApiService.GetShowsByIds(watchlistIds.Select(sw => sw.ShowId));
            var likedShows = await _jikanApiService.GetShowsByIds(likedShowsIds.Select(sl => sl.ShowId));
            var viewModel = new UserProfileViewModel(){
                User = user,
                Watchlist = watchlistShows,
                LikedShows = likedShows
            };
            return View(viewModel);
        }
        
        public async Task<IActionResult> GetWatchlist(){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var watchlist = await _context.ShowWatchlists.Where(sw => sw.ApplicationUserId == user.Id).ToListAsync();
            var shows = await _jikanApiService.GetShowsByIds(watchlist.Select(sw => sw.ShowId));
            return Json(shows);
        }
        public async Task<IActionResult> GetLikedShows(){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var likedShows = await _context.ShowLikes.Where(sl => sl.ApplicationUserId == user.Id).ToListAsync();
            var shows = await _jikanApiService.GetShowsByIds(likedShows.Select(sl => sl.ShowId));
            return Json(shows);
        }
    }
}
