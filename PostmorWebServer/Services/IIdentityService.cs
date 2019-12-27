using PostmorWebServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Services
{
    interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsyc(string Email, string Password);
        Task<AuthenticationResult> LoginAsyc(string Email, string Password);
        Task<AuthenticationResult> RefreshTokenAsyc(string token, string refreshToken);
    }
}
