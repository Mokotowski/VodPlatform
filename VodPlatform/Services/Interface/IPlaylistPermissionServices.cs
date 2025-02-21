using VodPlatform.Models;

namespace VodPlatform.Services.Interface
{
    public interface IPlaylistPermissionServices
    {
        public Task<List<UserPlaylistPermision>> GetUsers(string searchTerm);
        public Task<bool> AddRole(string userId, string role, List<int> Id_Series, string currentUserId);
        public Task<bool> RemoveRole(string userId, string role, List<int> Id_Series, string currentUserId);
    }
}
