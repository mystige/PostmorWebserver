using PostmorWebServer.Data.Entities;
using PostmorWebServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Services
{
    public interface IContactService
    {
        Task<bool> AddAsync(string Token, int ContactId);
        Task<bool> RemoveAsync(string Token, int ContactId);
        Task<UserCard> FindUserByIdAsync(string Token, int ContactId);
        Task<UserCard> FindUserByAddressAsync(string Token, string ContactAddres);
    }
}
