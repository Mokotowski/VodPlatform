﻿using VodPlatform.Database;

namespace VodPlatform.Services.Interface
{
    public interface IFunctionsFromEmail
    {
        public Task ConfirmedEmail(string code, string Email);

        public Task ResetPassword(string code, string Email, string password);

    }
}
