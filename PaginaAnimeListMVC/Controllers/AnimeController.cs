using Microsoft.AspNetCore.Mvc;
using PaginaAnimeListMVC.Models;
using PaginaAnimeListMVC.Services;
namespace PaginaAnimeListMVC.Controllers
{
    public class AnimeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly JikanApiService _jikanApiService;
        public AnimeController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _jikanApiService = new JikanApiService(httpClientFactory, config);
        }
        public async Task<IActionResult> Index()
        {
            //Pedir la informacion de todos los animes
            //Procesar la informacion que recibimos de la api
            //Empaquetar la info en objetos
            var shows = await _jikanApiService.GetTopShows();

            //Enviar los objetos a la vista
            return View(shows);
        }
        public IActionResult Detalles(int id)
        {
            //Pedir informacion acerca del show con Id = id;

            // crear un Show()
            var show = new Show()
            {
                Title = "One Piece",
                Description = "Monkey D. Luffy y su tripulación de piratas buscan el One Piece, el tesoro más grande del mundo.",
                Image = "https://m.media-amazon.com/images/I/81zQxy5mi8L._AC_SY679_.jpg",
                Genre = "Aventura, Acción, Fantasía",
                Studio = "Toei Animation",
                ReleaseDate = new DateTime(1999, 10, 20),
                Rating = 9
            };
            return View(show);
        }
    }
}
