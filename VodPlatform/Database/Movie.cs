using System.ComponentModel.DataAnnotations;

namespace VodPlatform.Database
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public int Id_Series { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }

        public Series Series { get; set; }
    }
}
