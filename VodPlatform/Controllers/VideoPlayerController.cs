using Microsoft.AspNetCore.Mvc;
using VodPlatform.Services;

namespace VodPlatform.Controllers
{
    public class VideoPlayerController : Controller
    {
        private readonly VideoServices _videoServices;

        public VideoPlayerController(VideoServices videoServices)
        {
            _videoServices = videoServices;
        }

 

        public async Task<IActionResult> MoviesList()
        {
            return View(await _videoServices.ListMovies());  
        }


        [HttpGet("watch/{movieName}")]
        public IActionResult WatchMovie(string movieName)
        {
            ViewBag.Movie = movieName;
            return View();
        }


        [HttpGet("stream/{movieName}/{file}")]
        public async Task<IActionResult> Stream(string movieName, string file)
        {
            return await _videoServices.SendData(movieName, file);
        }


    }
}
