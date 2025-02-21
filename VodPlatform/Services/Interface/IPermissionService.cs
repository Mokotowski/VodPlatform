using VodPlatform.Database;

namespace VodPlatform.Services.Interface
{
    public interface IPermissionService
    {
        Task<List<UserModel>> GetUsers(string searchTerm);
        public Task<bool> AddRole(string userId, string role, string currentUserId);
        public Task<bool> RemoveRole(string userId, string role, string currentUserId);
    }
}
