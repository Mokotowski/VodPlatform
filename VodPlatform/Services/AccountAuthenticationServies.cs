using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VodPlatform.Database;
using VodPlatform.Services.Interface;

namespace VodPlatform.Services
{
    public class AccountAuthenticationServies : IRegister, ILogin, ILogout
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly ILogger<AccountAuthenticationServies> _logger;

        public AccountAuthenticationServies(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, ILogger<AccountAuthenticationServies> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<bool> SimpleRegister(string Nick, string FirstName, string LastName, string PhoneNumber, string Email, string Password)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(Email);
            if (existingUserByEmail != null)
            {
                _logger.LogWarning("User with email {Email} already exists.", Email);
                return false;
            }

            bool phoneNumberExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == PhoneNumber);
            if (phoneNumberExists)
            {
                _logger.LogWarning("User with phone number {PhoneNumber} already exists.", PhoneNumber);
                return false;
            }

            var user = new UserModel
            {
                Firstname = FirstName,
                Lastname = LastName,
                UserName = Nick,
                Email = Email,
                PhoneNumber = PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created successfully with Name: {FirstName} {LastName}.", Email, FirstName, LastName);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to create user {Email}. Errors: {Errors}", Email, string.Join(", ", result.Errors));
                return false;
            }
        }

        public async Task<bool> SimpleLogin(string Nick, string Password)
        {
            _logger.LogInformation("Login attempt for user: {Nick}", Nick);

            var result = await _signInManager.PasswordSignInAsync(Nick, Password, true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Nick} successfully logged in.", Nick);
                return true;

            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Nick} is locked out due to too many failed login attempts.", Nick);
                return false;
            }
            else if (result.IsNotAllowed)
            {
                _logger.LogWarning("Login attempt by user {Nick} is not allowed.", Nick);
                return false;
            }
            else if (result.RequiresTwoFactor)
            {
                _logger.LogWarning("User {Nick} requires two-factor authentication.", Nick);
                return false;
            }
            else
            {
                _logger.LogWarning("Failed login attempt for user: {Nick}", Nick);
                _logger.LogWarning("Sign-in result: {Result}", result);
                return false;
            }
        }



        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
