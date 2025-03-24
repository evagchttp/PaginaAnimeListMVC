using PaginaAnimeListMVC.Services.Interfaces;
using PaginaAnimeListMVC.Models;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace PaginaAnimeListMVC.Services
{
    public class JikanApiService : IJikanApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromHours(1); // Default cache of 1 hour

        public JikanApiService(IHttpClientFactory httpClientFactory, IConfiguration config, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _cache = cache;
            _httpClient = _httpClientFactory.CreateClient();
        }
        public async Task<Show> GetShowById(int id){
            string cacheKey = $"Show_{id}";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out Show cachedShow))
            {
                return cachedShow;
            }
            
            // If not in cache, get from API
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
                        
                        // Cache the result
                        _cache.Set(cacheKey, show, _defaultCacheDuration);
                        
                        return show;
                    }
                }
            }
            return show;
        }
        public async Task<List<Show>> GetShowsByIds(IEnumerable<int> ids)
        {
            string cacheKey = $"Shows_{string.Join("_", ids)}";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out List<Show> cachedShows))
            {
                return cachedShows;
            }

            List<Show> shows = new List<Show>();

            foreach (var id in ids)
            {
                var show = await GetShowById(id);
                if (show != null)
                {
                    shows.Add(show);
                }
            }
            
            // Cache the result
            _cache.Set(cacheKey, shows, _defaultCacheDuration);

            return shows;
        }
        public async Task<List<Show>> GetTopShows()
        {
            string cacheKey = "TopShows";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out List<Show> cachedShows))
            {
                return cachedShows;
            }
            
            List<Show> shows = new List<Show>();
            string? baseUrl = _config["JikanAnime:BaseUrl"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                var responseMessage = await _httpClient.GetAsync(baseUrl + "/top/anime");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    var jsonData = JsonSerializer.Deserialize<JikanResponse>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });

                    if (jsonData?.Data != null)
                    {
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
                        
                        // Cache the result
                        _cache.Set(cacheKey, shows, _defaultCacheDuration);
                    }
                }
            }

            return shows;
        }
        public async Task<List<Show>> GetShowsBySearch(
            string query,
            int page = 1,
            int limit = 10,
            string type = "tv",
            bool sfw = true,
            List<string> genres = null
            ){
            string cacheKey = $"Search_{query}_{page}_{limit}_{type}_{sfw}_{(genres != null ? string.Join("_", genres) : "noGenres")}";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out List<Show> cachedShows))
            {
                return cachedShows;
            }
            
            List<Show> shows = new List<Show>();
            string? baseUrl = _config["JikanAnime:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl)){
                var queryParams = new Dictionary<string, string>
                {
                    {"q", query},
                    {"page", page.ToString()},
                    {"limit", limit.ToString()},
                    {"type", type},
                    {"sfw", sfw.ToString().ToLower()},
                    {"genres", genres != null ? string.Join(",", genres) : string.Empty}
                };
                var queryString = new FormUrlEncodedContent(queryParams);
                var responseMessage = await _httpClient.GetAsync(baseUrl + "/search/anime?" + await queryString.ReadAsStringAsync());
                if (responseMessage.IsSuccessStatusCode){
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    var jsonData = JsonSerializer.Deserialize<JikanResponse>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });
                    if (jsonData?.Data != null){
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
                        
                        // Cache the result with a shorter duration for search results
                        _cache.Set(cacheKey, shows, TimeSpan.FromMinutes(30));
                    }
                }
            }
            return shows;
        }
        public async Task<List<Show>> GetRelatedShows(int id, int limit = 12){
            string cacheKey = $"RelatedShows_{id}_{limit}";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out List<Show> cachedShows))
            {
                return cachedShows;
            }
            
            List<Show> shows = new List<Show>();
            string? baseUrl = _config["JikanAnime:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl)){
                var responseMessage = await _httpClient.GetAsync(baseUrl + "/anime/" + id + "/recommendations");
                if (responseMessage.IsSuccessStatusCode){
                    var jsonString = await responseMessage.Content.ReadAsStringAsync();
                    var jsonData = JsonSerializer.Deserialize<JikanResponseRecommendations>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });
                    if (jsonData?.Data != null){
                        shows = jsonData.Data.Select(item => new Show
                        {
                            Id = item.Entry.MalId,
                            Title = item.Entry.Title,
                            Image = item.Entry.Images?.Jpg?.LargeImageUrl ?? string.Empty,
                        }).ToList().Take(limit).ToList();
                        
                        // Cache the result
                        _cache.Set(cacheKey, shows, _defaultCacheDuration);
                    }
                }
            }
            return shows;
        }
        public class JikanResponse
        {
            public List<JikanAnime> Data { get; set; }
        }
        public class JikanResponseRecommendations
        {
            public List<RecommendationItem> Data { get; set; }
        }

        public class RecommendationItem
        {
            public Entry Entry { get; set; }
        }

        public class Entry 
        {
            public int MalId { get; set; }
            public string Title { get; set; }
            public JikanImages Images { get; set; }
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
