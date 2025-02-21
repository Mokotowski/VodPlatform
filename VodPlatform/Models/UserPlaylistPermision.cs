using VodPlatform.Database;

namespace VodPlatform.Models
{
    public class UserPlaylistPermision
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public IList<string> Roles {  get; set; } 
        public List<PlaylistPermision> PlaylistsPermission { get; set; }

    }
}
