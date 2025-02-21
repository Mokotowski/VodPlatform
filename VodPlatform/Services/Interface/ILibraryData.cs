using VodPlatform.Database;

namespace VodPlatform.Services.Interface
{
    public interface ILibraryData
    {
        public Task<List<Movie>> GetAllMovies();
        public Task<List<Series>> GetAllSeries();
    }
}
