using PostmorWebServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string Email, string Password, string Name, string Adress, string Picture);
        Task<AuthenticationResult> LoginAsync(string Email, string Password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        Task<RegisterResult> GenerateUserRegisterResponseAsync(int Id);
        Task<FetchAllResult> FetchAllAsync(string token);
        Task<List<string>> GenerateAddresses(int amount);
        Task<bool> ChangePasswordAsync(string token, string password, string newPassword);

    }
}
