using PostmorWebServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsyc(string Email, string Password, string Name, string Adress, string Picture);
        Task<AuthenticationResult> LoginAsyc(string Email, string Password);
        Task<AuthenticationResult> RefreshTokenAsyc(string token, string refreshToken);
    }
}
