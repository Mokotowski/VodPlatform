using Microsoft.AspNetCore.Mvc;

namespace VodPlatform.Services.Interface
{
    public interface ILibraryActions
    {
        public Task AddMovie(int Id_Series, string Title, string Type, string Category);
        public Task AddSerial(int Seasons, string Title, string Category, List<int> Episodes, int Id_Series);
        public Task AddSeries(string Title);
        public Task DataAssociation(int Id_Series, List<int> Id_Movie);

    }
}
