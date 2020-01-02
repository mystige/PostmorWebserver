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
        Task<bool> AddAsync(int RequesterId, int ContactId);
        Task<bool> RemoveAsync(int RequesterId, int ContactId);
        Task<User> FindUserByIdAsync(int Id);
        Task<User> FindUserByAddressAsync(string Address);
    }
}
