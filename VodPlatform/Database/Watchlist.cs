using System.ComponentModel.DataAnnotations;

namespace VodPlatform.Database
{
    public class Watchlist
    {
        [Key]
        public int Id { get; set; }
        public string Id_User { get; set; }
        public string Title { get; set; }
        public int Id_Series { get; set; }
        public int Id_Movie { get; set; }

        public UserModel User { get; set; }
    }
}
