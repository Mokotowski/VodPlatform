using System.ComponentModel.DataAnnotations;

namespace VodPlatform.Database
{
    public class Series
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Movie> Movies { get; set; }

        public List<PlaylistPermision> PlaylistPermision { get; set; }
    }
}
