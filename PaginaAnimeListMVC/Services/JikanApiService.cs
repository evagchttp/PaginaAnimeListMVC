using PaginaAnimeListMVC.Services.Interfaces;
using PaginaAnimeListMVC.Models;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace PaginaAnimeListMVC.Services
{
    public class JikanApiService : IJikanApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;
        private readonly IConfiguration _config;
        public JikanApiService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;

            _httpClient = _httpClientFactory.CreateClient();

        }
        public async Task<Show> GetShowById(int id){
            Show show = new Show();
            string? baseUrl = _config["JikanAnime:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                var responseMessage = await _httpClient.GetAsync(baseUrl + "/anime/" + id);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    var jsonData = JsonSerializer.Deserialize<JikanResponseSingle>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });
                    if (jsonData != null)
                    {
                        show = new Show
                        {
                            Id = jsonData.Data.MalId,
                            Title = jsonData.Data.Title,
                            Description = jsonData.Data.Synopsis ?? "No description available.",
                            Image = jsonData.Data.Images?.Jpg?.LargeImageUrl ?? string.Empty,
                            Genre = jsonData.Data.Genres?.FirstOrDefault()?.Name ?? "Unknown",
                            Studio = jsonData.Data.Studios?.FirstOrDefault()?.Name ?? "Unknown",
                            ReleaseDate = DateTime.TryParse(jsonData.Data.Aired?.From, out var date) ? date : DateTime.MinValue,
                            Rating = (int)(jsonData.Data.Score ?? 0)
                        };
                        return show;
                    }
                }
            }
            return show;
        }
        public async Task<List<Show>> GetTopShows()
        {
            List<Show> shows = new List<Show>();
            string? baseUrl = _config["JikanAnime:BaseUrl"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                var responseMessage = await _httpClient.GetAsync(baseUrl + "/top/anime");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    var jsonData = JsonSerializer.Deserialize<JikanResponseTop>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });

                    if (jsonData?.Data != null)
                    {
                        Console.WriteLine("JsonDataImageUrl:" + jsonData.Data[0].Images.Jpg.LargeImageUrl);
                        shows = jsonData.Data.Select(item => new Show
                        {
                            Id = item.MalId,
                            Title = item.Title,
                            Description = item.Synopsis ?? "No description available.",
                            Image = item.Images?.Jpg?.LargeImageUrl ?? string.Empty,
                            Genre = item.Genres?.FirstOrDefault()?.Name ?? "Unknown",
                            Studio = item.Studios?.FirstOrDefault()?.Name ?? "Unknown",
                            ReleaseDate = DateTime.TryParse(item.Aired?.From, out var date) ? date : DateTime.MinValue,
                            Rating = (int)(item.Score ?? 0)
                        }).ToList();
                        Console.WriteLine("imageUrl:" + shows[0].Image);
                    }
                }
            }

            return shows;
        }

        public class JikanResponseTop
        {
            public List<JikanAnime> Data { get; set; }
        }
        public class JikanResponseSingle
        {
            public JikanAnime Data { get; set; }
        }

        public class JikanAnime
        {
            public int MalId { get; set; }
            public string Title { get; set; }
            public string Synopsis { get; set; }
            public JikanImages Images { get; set; }
            public List<JikanGenre> Genres { get; set; }
            public List<JikanStudio> Studios { get; set; }
            public JikanAired Aired { get; set; }
            public double? Score { get; set; }
        }

        public class JikanImages
        {
            public JikanImage Jpg { get; set; }
        }

        public class JikanImage
        {
            public string LargeImageUrl { get; set; }
        }

        public class JikanGenre
        {
            public string Name { get; set; }
        }

        public class JikanStudio
        {
            public string Name { get; set; }
        }

        public class JikanAired
        {
            public string From { get; set; }
        }
    }
}
