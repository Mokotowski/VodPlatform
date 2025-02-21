using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VodPlatform.Database;
using VodPlatform.Models;
using VodPlatform.Services.Interface;

namespace VodPlatform.Services
{
    public class PlaylistPermissionServices : IPlaylistPermissionServices
    {
        private readonly ILogger<PlaylistPermissionServices> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseContext _context;

        public PlaylistPermissionServices(ILogger<PlaylistPermissionServices> logger, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, DatabaseContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<UserPlaylistPermision>> GetUsers(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Fetching users. Search term: {SearchTerm}", searchTerm);

                var users = _userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    users = users.Where(u => u.UserName.Contains(searchTerm) ||
                                             u.Firstname.Contains(searchTerm) ||
                                             u.Lastname.Contains(searchTerm) ||
                                             u.Email.Contains(searchTerm));
                }

                List<UserPlaylistPermision> userPermissions = new List<UserPlaylistPermision>();

                foreach (var user in users)
                {
                    UserPlaylistPermision userPermission = new UserPlaylistPermision()
                    {
                        Id = user.Id,
                        Firstname = user.Firstname,
                        Lastname = user.Lastname,
                        Email = user.Email,
                        Roles = await _userManager.GetRolesAsync(user),
                        PlaylistsPermission = await _context.PlaylistPermision.Where(p => p.Id_User == user.Id).ToListAsync()
                    };
                    userPermissions.Add(userPermission);
                }

                _logger.LogInformation("Fetched {Count} users successfully.", userPermissions.Count);
                return userPermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users.");
                return new List<UserPlaylistPermision>();
            }
        }

        public async Task<bool> AddRole(string userId, string role, List<int> Id_Series, string currentUserId)
        {
            try
            {
                _logger.LogInformation("Adding role {Role} to user {UserId} by {CurrentUserId}.", role, userId, currentUserId);

                var user = await _userManager.FindByIdAsync(userId);
                var currentUser = await _userManager.FindByIdAsync(currentUserId);

                if (user == null || currentUser == null)
                {
                    _logger.LogWarning("User {UserId} or {CurrentUserId} not found.", userId, currentUserId);
                    return false;
                }

                var currentRoles = await _userManager.GetRolesAsync(currentUser);
                if (!currentRoles.Contains("SuperAdmin") && !currentRoles.Contains("Admin") && !currentRoles.Contains("Moderator"))
                {
                    _logger.LogWarning("User {CurrentUserId} does not have permission to assign roles.", currentUserId);
                    return false;
                }

                if (await _userManager.IsInRoleAsync(user, role))
                {
                    _logger.LogWarning("User {UserId} already has the role {Role}.", userId, role);
                    return false;
                }

                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to add role {Role} to user {UserId}.", role, userId);
                    return false;
                }

                if (Id_Series != null && Id_Series.Count > 0)
                {
                    List<PlaylistPermision> permissions = new List<PlaylistPermision>();

                    foreach (int series in Id_Series)
                    {
                        Series seriesDb = await _context.Series.FindAsync(series);
                        bool permissionExists = await _context.PlaylistPermision.AnyAsync(p => p.Id_User == user.Id && p.Id_Series == series);
                        if (!permissionExists)
                        {
                            PlaylistPermision newPermission = new PlaylistPermision()
                            {
                                Id_User = user.Id,
                                Id_Series = series,
                                Name = seriesDb.Title,
                                Series = seriesDb
                            };

                            permissions.Add(newPermission);
                        }
                    }
                    await _context.PlaylistPermision.AddRangeAsync(permissions);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Successfully added role {Role} to user {UserId}.", role, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding role {Role} to user {UserId}.", role, userId);
                return false;
            }
        }

        public async Task<bool> RemoveRole(string userId, string role, List<int> Id_Series, string currentUserId)
        {
            try
            {
                _logger.LogInformation("Removing role {Role} from user {UserId} by {CurrentUserId}.", role, userId, currentUserId);

                var user = await _userManager.FindByIdAsync(userId);
                var currentUser = await _userManager.FindByIdAsync(currentUserId);

                if (user == null || currentUser == null)
                {
                    _logger.LogWarning("User {UserId} or {CurrentUserId} not found.", userId, currentUserId);
                    return false;
                }

                var currentRoles = await _userManager.GetRolesAsync(currentUser);
                if (!currentRoles.Contains("SuperAdmin") && !currentRoles.Contains("Admin") && !currentRoles.Contains("Moderator"))
                {
                    _logger.LogWarning("User {CurrentUserId} does not have permission to remove roles.", currentUserId);
                    return false;
                }

                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    _logger.LogWarning("User {UserId} does not have the role {Role}.", userId, role);
                    return false;
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to remove role {Role} from user {UserId}.", role, userId);
                    return false;
                }

                if (Id_Series != null && Id_Series.Count > 0)
                {
                    List<PlaylistPermision> permissions = await _context.PlaylistPermision
                        .Where(p => p.Id_User == user.Id && Id_Series.Contains(p.Id_Series))
                        .ToListAsync();

                    _context.PlaylistPermision.RemoveRange(permissions);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Successfully removed role {Role} from user {UserId}.", role, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing role {Role} from user {UserId}.", role, userId);
                return false;
            }
        }
    }
}
