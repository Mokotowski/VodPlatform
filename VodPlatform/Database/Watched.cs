using System.ComponentModel.DataAnnotations;

namespace VodPlatform.Database
{
    public class Watched
    {
        [Key]
        public int Id { get; set; }
        public string Id_User { get; set; } 
        public int Id_Movie { get; set; }
        public int Id_Series { get; set; }
        public bool Completed { get; set; }
        public string Title { get; set; }
        public DateTime When { get; set; }

        public UserModel User { get; set; }
    }
}
