using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VodPlatform.Database;
using VodPlatform.Services;
using VodPlatform.Services.Interface;

namespace VodPlatform.Controllers
{
    [Authorize]
    public class AdminPanelController : Controller
    {
        private readonly ILibraryActions _libraryActions;
        private readonly ILibraryData _libraryData;
        private readonly IPermissionService _permissionService;
        private readonly IPlaylistPermissionServices _playlistPermissionServices;
        private readonly UserManager<UserModel> _userManager;
        public AdminPanelController(ILibraryActions libraryActions, ILibraryData libraryData, IPermissionService permissionService, IPlaylistPermissionServices playlistPermissionServices, UserManager<UserModel> userManager)
        {
            _libraryActions = libraryActions;
            _libraryData = libraryData;
            _permissionService = permissionService;
            _playlistPermissionServices = playlistPermissionServices;
            _userManager = userManager; 
        }


        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            ViewBag.UserRole = roles.Contains("SuperAdmin") ? "SuperAdmin" : "Admin";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers(string searchTerm)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin") && !roles.Contains("Moderator"))
            {
                return Forbid();
            }

            var users = await _permissionService.GetUsers(searchTerm);

            var filteredUsers = users.Select(u => new
            {
                u.Id,
                u.Firstname,
                u.Lastname,
                u.UserName,
                u.Email
            });

            return Json(filteredUsers);
        }


        [HttpPost]
        public async Task<IActionResult> AddPermission(string userId, string role)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);

            if (!currentRoles.Contains("Admin") && !currentRoles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            if (role == "Admin" && !currentRoles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            var success = await _permissionService.AddRole(userId, role, currentUser.Id);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> RemovePermission(string userId, string role)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);

            if (!currentRoles.Contains("Admin") && !currentRoles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            if (role == "Admin" && !currentRoles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            var success = await _permissionService.RemoveRole(userId, role, currentUser.Id);
            return Json(new { success });
        }






        [HttpGet]
        public async Task<IActionResult> ManageLibrary()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && !roles.Contains("SuperAdmin"))
            {
                return Forbid(); 
            }


            List<Movie> movies = await _libraryData.GetAllMovies();
            List<Series> series = await _libraryData.GetAllSeries();

            ViewBag.Movies = movies;
            ViewBag.Series = series;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(int Id_Series, string Title, string Type, string Category)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && !roles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            await _libraryActions.AddMovie(Id_Series, Title, Type, Category);
            return RedirectToAction("ManageLibrary", "AdminPanel");
        }

        [HttpPost]
        public async Task<IActionResult> AddSerial(int Seasons, string Title, string Category, List<int> Episodes, int Id_Series)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && !roles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            await _libraryActions.AddSerial(Seasons, Title, Category, Episodes, Id_Series);
            return RedirectToAction("ManageLibrary", "AdminPanel");
        }


        [HttpPost]
        public async Task<IActionResult> AddSeries(string Title)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && !roles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            _libraryActions.AddSeries(Title);
            return RedirectToAction("ManageLibrary", "AdminPanel");
        }

        [HttpPost]
        public async Task<IActionResult> MovieAssociation(int Id_Series, List<int> Id_Movie)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && !roles.Contains("SuperAdmin"))
            {
                return Forbid();
            }

            await _libraryActions.DataAssociation(Id_Series, Id_Movie);
            return RedirectToAction("ManageLibrary", "AdminPanel");
        }






        [HttpGet]
        public async Task<IActionResult> ManageUsersPlaylist()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersPermisionPlaylist(string searchTerm)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin") && !roles.Contains("Moderator"))
            {
                return Forbid();
            }

            var users = await _playlistPermissionServices.GetUsers(searchTerm);

            return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddPermisonPlaylist(string role, List<int> Id_Series, string currentUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin") && !roles.Contains("Moderator"))
            {
                return Forbid();
            }

            _playlistPermissionServices.AddRole(user.Id, role, Id_Series, currentUserId);

            return RedirectToAction("ManageUsersPlaylist", "AdminPanel");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePermisonPlaylist(string role, List<int> Id_Series, string currentUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin") && !roles.Contains("Moderator"))
            {
                return Forbid();
            }

            _playlistPermissionServices.RemoveRole(user.Id, role, Id_Series, currentUserId);

            return RedirectToAction("ManageUsersPlaylist", "AdminPanel");
        }
    }
}
