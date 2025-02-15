using VodPlatform.Database;

namespace VodPlatform.Services.Interface
{
    public interface ISendEmail
    {
        public Task SendConfirmedEmail(string Email);
        public Task SendResetPasswordEmail(string EMail);
    }
}
