using Microsoft.AspNetCore.Identity;

namespace VodPlatform.Database
{
    public class UserModel : IdentityUser
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}
