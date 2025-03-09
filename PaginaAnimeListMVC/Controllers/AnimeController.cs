using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PaginaAnimeListMVC.Models;


namespace PaginaAnimeListMVC.Controllers
{
    public class AnimeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        
        public AnimeController(IHttpClientFactory httpClientFactory, IConfiguration config){
            _httpClientFactory = httpClientFactory;
            _config = config;
        }
        public IActionResult Index()
        {
            //Pedir la informacion de todos los animes
            //Procesar la informacion que recibimos de la api
            //Empaquetar la info en objetos
            //Enviar los objetos a la vista

            var httpClient = _httpClientFactory.CreateClient();
            string baseUrl = _config["JikanAnime:BaseUrl"];
            Console.WriteLine($"Base URL: {baseUrl}");
            if(!baseUrl.IsNullOrEmpty()){
                httpClient.GetAsync(baseUrl+"/top/anime");
            }
            

            var listaDeAnimes = new List<Show>()
            {
                new Show()
                {
                    Title = "One Piece",
                    Description = "Monkey D. Luffy y su tripulación de piratas buscan el One Piece, el tesoro más grande del mundo.",
                    Image = "https://m.media-amazon.com/images/I/81zQxy5mi8L._AC_SY679_.jpg",
                    Genre = "Aventura, Acción, Fantasía",
                    Studio = "Toei Animation",
                    ReleaseDate = new DateTime(1999, 10, 20),
                    Rating = 9
                },
                new Show()
                {
                    Title = "Naruto",
                    Description = "Naruto Uzumaki, un joven ninja, sueña con convertirse en el líder de su aldea y ganarse el respeto de todos.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4f/Naruto_logo.svg/600px-Naruto_logo.svg.png",
                    Genre = "Acción, Aventura, Drama",
                    Studio = "Studio Pierrot",
                    ReleaseDate = new DateTime(2002, 10, 3),
                    Rating = 8
                },
                new Show()
                {
                    Title = "Attack on Titan",
                    Description = "En un mundo donde la humanidad vive cercada por enormes muros, los Titanes amenazan con exterminarlos.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Shingeki_no_Kyojin_logo.svg",
                    Genre = "Acción, Drama, Fantasía",
                    Studio = "Wit Studio, MAPPA",
                    ReleaseDate = new DateTime(2013, 4, 7),
                    Rating = 9
                },
                new Show()
                {
                    Title = "My Hero Academia",
                    Description = "En un mundo donde la mayoría de las personas tiene poderes especiales, Izuku Midoriya lucha por convertirse en un héroe.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/My_Hero_Academia_Logo.svg/1280px-My_Hero_Academia_Logo.svg.png",
                    Genre = "Acción, Superhéroes, Escuela",
                    Studio = "Bones",
                    ReleaseDate = new DateTime(2016, 4, 3),
                    Rating = 8
                },
                new Show()
                {
                    Title = "Demon Slayer",
                    Description = "Tanjiro Kamado se convierte en un cazador de demonios después de que su familia fuera asesinada por estos y su hermana transformada en demonio.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/6/6d/Demon_Slayer_Logo.svg",
                    Genre = "Acción, Fantasía, Horror",
                    Studio = "ufotable",
                    ReleaseDate = new DateTime(2019, 4, 6),
                    Rating = 9
                },
                new Show()
                {
                    Title = "Fullmetal Alchemist: Brotherhood",
                    Description = "Los hermanos Elric intentan recuperar lo que perdieron al intentar usar la alquimia para traer a su madre de vuelta a la vida.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/e/e5/Fullmetal_Alchemist_Brotherhood_logo.png",
                    Genre = "Acción, Aventura, Fantasía",
                    Studio = "Studio Bones",
                    ReleaseDate = new DateTime(2009, 4, 5),
                    Rating = 10
                },
                new Show()
                {
                    Title = "Death Note",
                    Description = "Light Yagami, un joven con un cuaderno que mata a cualquiera cuyo nombre escriba, se enfrenta a un misterioso detective llamado L.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/3/39/Death_Note_logo.svg",
                    Genre = "Psicológico, Thriller, Sobrenatural",
                    Studio = "Madhouse",
                    ReleaseDate = new DateTime(2006, 10, 3),
                    Rating = 9
                },
                new Show()
                {
                    Title = "Dragon Ball Z",
                    Description = "Goku y sus amigos luchan contra poderosos enemigos para proteger la Tierra y las esferas del dragón.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c2/Dragon_Ball_Z_logo.svg/1200px-Dragon_Ball_Z_logo.svg.png",
                    Genre = "Acción, Aventura, Artes marciales",
                    Studio = "Toei Animation",
                    ReleaseDate = new DateTime(1989, 4, 26),
                    Rating = 8
                },
                new Show()
                {
                    Title = "Sword Art Online",
                    Description = "Los jugadores de un videojuego de realidad virtual se ven atrapados en el juego y deben luchar para salir.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/6/6d/Sword_Art_Online_Logo.svg",
                    Genre = "Acción, Aventura, Ciencia ficción",
                    Studio = "A-1 Pictures",
                    ReleaseDate = new DateTime(2012, 7, 8),
                    Rating = 7
                },
                new Show()
                {
                    Title = "Tokyo Ghoul",
                    Description = "Ken Kaneki se convierte en un ghoul después de un accidente, y debe adaptarse a su nueva vida mientras lucha con su humanidad.",
                    Image = "https://upload.wikimedia.org/wikipedia/commons/6/64/Tokyo_Ghoul_Logo.svg",
                    Genre = "Horror, Acción, Sobrenatural",
                    Studio = "Studio Pierrot",
                    ReleaseDate = new DateTime(2014, 7, 4),
                    Rating = 8
                }
            };
            
            return View(listaDeAnimes);
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
         
