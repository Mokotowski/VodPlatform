using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VodPlatform.Database;
using VodPlatform.Services.Interface;

namespace VodPlatform.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionService(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, ILogger<PermissionService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<UserModel>> GetUsers(string searchTerm)
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

                var result = await users.Select(u => new UserModel
                {
                    Id = u.Id,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToListAsync();

                _logger.LogInformation("Fetched {Count} users successfully.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users.");
                return new List<UserModel>();
            }
        }

        public async Task<bool> AddRole(string userId, string role, string currentUserId)
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
                if (!currentRoles.Contains("SuperAdmin") && !currentRoles.Contains("Admin"))
                {
                    _logger.LogWarning("User {CurrentUserId} does not have permission to add roles.", currentUserId);
                    return false;
                }

                if (role == "Admin" && !currentRoles.Contains("SuperAdmin"))
                {
                    _logger.LogWarning("User {CurrentUserId} is not a SuperAdmin, cannot assign Admin role.", currentUserId);
                    return false;
                }

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogWarning("Role {Role} does not exist.", role);
                    return false;
                }

                if (await _userManager.IsInRoleAsync(user, role))
                {
                    _logger.LogWarning("User {UserId} already has the role {Role}.", userId, role);
                    return false;
                }

                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully added role {Role} to user {UserId}.", role, userId);
                }
                else
                {
                    _logger.LogError("Failed to add role {Role} to user {UserId}. Errors: {Errors}", role, userId, result.Errors);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding role {Role} to user {UserId}.", role, userId);
                return false;
            }
        }

        public async Task<bool> RemoveRole(string userId, string role, string currentUserId)
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
                if (!currentRoles.Contains("SuperAdmin") && !currentRoles.Contains("Admin"))
                {
                    _logger.LogWarning("User {CurrentUserId} does not have permission to remove roles.", currentUserId);
                    return false;
                }

                if (role == "Admin" && !currentRoles.Contains("SuperAdmin"))
                {
                    _logger.LogWarning("User {CurrentUserId} is not a SuperAdmin, cannot remove Admin role.", currentUserId);
                    return false;
                }

                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    _logger.LogWarning("User {UserId} does not have the role {Role}.", userId, role);
                    return false;
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully removed role {Role} from user {UserId}.", role, userId);
                }
                else
                {
                    _logger.LogError("Failed to remove role {Role} from user {UserId}. Errors: {Errors}", role, userId, result.Errors);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing role {Role} from user {UserId}.", role, userId);
                return false;
            }
        }
    }
}
