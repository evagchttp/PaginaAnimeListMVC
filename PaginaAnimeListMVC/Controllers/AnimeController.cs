using Microsoft.AspNetCore.Mvc;
using PaginaAnimeListMVC.Models;
using PaginaAnimeListMVC.Services;
using Microsoft.EntityFrameworkCore;
using PaginaAnimeListMVC.Data;
namespace PaginaAnimeListMVC.Controllers
{
    public class AnimeController : Controller
    {
        private readonly JikanApiService _jikanApiService;
        private readonly ApplicationDbContext _context;
        public AnimeController(IHttpClientFactory httpClientFactory, IConfiguration config, ApplicationDbContext context)
        {
            _jikanApiService = new JikanApiService(httpClientFactory, config);
            _context = context;
        }
        [Route("/")]
        [Route("/Home")]
        [Route("/Anime/Index")]
        [Route("/Anime")]
        public async Task<IActionResult> Index()
        {
            //Pedir la informacion de todos los animes
            //Procesar la informacion que recibimos de la api
            //Empaquetar la info en objetos
            var shows = await _jikanApiService.GetTopShows();

            //Enviar los objetos a la vista
            return View(shows);
        }
        public async Task<IActionResult> Detalle(int id)
        {
            //Pedir informacion acerca del show con Id = id;
            var show = await _jikanApiService.GetShowById(id);
            
            // Check if the user has liked or added the show to their watchlist
            ViewBag.IsLiked = await IsShowLikedByCurrentUser(id);
            ViewBag.IsInWatchlist = await IsShowInCurrentUserWatchlist(id);
            
            return View(show);
        }
        
        // Private method to check if a show is liked by the current user
        private async Task<bool> IsShowLikedByCurrentUser(int showId)
        {
            var userName = HttpContext.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }
            
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return false;
            }
            
            var show = await _jikanApiService.GetShowById(showId);
            if (show == null)
            {
                return false;
            }
            
            return _context.ShowLikes.Any(sl => sl.ShowId == show.Id && sl.ApplicationUserId == user.Id);
        }
        
        // Private method to check if a show is in the current user's watchlist
        private async Task<bool> IsShowInCurrentUserWatchlist(int showId)
        {
            var userName = HttpContext.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }
            
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return false;
            }
            
            var show = await _jikanApiService.GetShowById(showId);
            if (show == null)
            {
                return false;
            }
            
            return _context.ShowWatchlists.Any(sw => sw.ShowId == show.Id && sw.ApplicationUserId == user.Id);
        }
        
        public async Task<IActionResult> Related(int id){
            var shows = await _jikanApiService.GetRelatedShows(id);
            if (shows.Count() > 0){
                return Json(shows);
            }
            return Json(new List<Show>());
        }
        public async Task<IActionResult> Like(int id){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var show = await _jikanApiService.GetShowById(id);
            if (show == null){
                //return Json(new { success = false, message = "Show not found" });
                return NotFound();
            }
            var isLiked = _context.ShowLikes.Any(sl => sl.ShowId == show.Id && sl.ApplicationUserId == user.Id);
            if (!isLiked)
            { 
                _context.ShowLikes.Add(new ShowLike()
                {
                    ApplicationUserId = user.Id,
                    ShowId = show.Id
                });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> Unlike(int id){
           var userName = HttpContext.User?.Identity?.Name;
           if (userName == null){
               return Unauthorized();
           }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var show = await _jikanApiService.GetShowById(id);
            if (show == null){
                return NotFound();
            }
            var isLiked = _context.ShowLikes.Any(sl => sl.ShowId == show.Id && sl.ApplicationUserId == user.Id);
            if (isLiked)
            { 
                _context.ShowLikes.Remove(new ShowLike()
                {
                    ApplicationUserId = user.Id,
                    ShowId = show.Id
                });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        public async Task<IActionResult> AddToWatchlist(int id){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var show = await _jikanApiService.GetShowById(id);
            if (show == null){
                return NotFound();
            }
            var isInWatchlist = _context.ShowWatchlists.Any(sw => sw.ShowId == show.Id && sw.ApplicationUserId == user.Id);
            if (!isInWatchlist){
                _context.ShowWatchlists.Add(new ShowWatchlist()
                {
                    ApplicationUserId = user.Id,
                    ShowId = show.Id
                });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        public async Task<IActionResult> RemoveFromWatchlist(int id){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var show = await _jikanApiService.GetShowById(id);
            if (show == null){
                return NotFound();
            }
            var isInWatchlist = _context.ShowWatchlists.Any(sw => sw.ShowId == show.Id && sw.ApplicationUserId == user.Id);
            if (isInWatchlist){
                _context.ShowWatchlists.Remove(new ShowWatchlist()
                {
                    ApplicationUserId = user.Id,
                    ShowId = show.Id
                });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
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
        
        // API endpoint for checking if a show is liked by the current user
        public async Task<IActionResult> IsLiked(int id){
            if (string.IsNullOrEmpty(HttpContext.User?.Identity?.Name))
            {
                return Unauthorized();
            }
            
            bool isLiked = await IsShowLikedByCurrentUser(id);
            return Json(new { isLiked });
        }
        
        // API endpoint for checking if a show is in the current user's watchlist
        public async Task<IActionResult> IsInWatchlist(int id){
            if (string.IsNullOrEmpty(HttpContext.User?.Identity?.Name))
            {
                return Unauthorized();
            }
            
            bool isInWatchlist = await IsShowInCurrentUserWatchlist(id);
            return Json(new { isInWatchlist });
        }
        
        public async Task<IActionResult> GetWatchlistCount(){
            var userName = HttpContext.User?.Identity?.Name;
            if (userName == null){
                return Unauthorized();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null){
                return Unauthorized();
            }
            var count = await _context.ShowWatchlists.CountAsync(sw => sw.ApplicationUserId == user.Id);
            return Json(new { count = count });
        }
    }
}

