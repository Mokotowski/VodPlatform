using Microsoft.AspNetCore.Mvc;

namespace VodPlatform.Services.Interface
{
    public interface IVideoStream
    {
        public Task<FileStreamResult> SendData(string movieName, string file);
        public Task<List<string>> ListMovies(); // Tymczasowa metoda
    }
}
