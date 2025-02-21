using System.ComponentModel.DataAnnotations;

namespace VodPlatform.Database
{
    public class PlaylistPermision
    {
        [Key]
        public int Id { get; set; }
        public string Id_User { get; set; }
        public int Id_Series { get; set; }
        public string Name { get; set; }

        public Series Series { get; set; }
    }
}
