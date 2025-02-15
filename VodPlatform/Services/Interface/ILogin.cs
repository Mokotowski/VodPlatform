namespace VodPlatform.Services.Interface
{
    public interface ILogin
    {
        public Task<bool> SimpleLogin(string Nick, string Password);
    }
}
