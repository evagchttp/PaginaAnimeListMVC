﻿using System.Threading.Tasks;
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
        public async Task<IActionResult> Detalle(int id)
        {
            //Pedir informacion acerca del show con Id = id;
            var show = await _jikanApiService.GetShowById(id);
            return View(show);
        }
    }
}
