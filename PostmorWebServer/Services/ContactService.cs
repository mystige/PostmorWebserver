using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PostmorWebServer.Data;
using PostmorWebServer.Domain;

namespace PostmorWebServer.Services
{
    public class ContactService : IContactService
    {
        private readonly DataContext _dbContext;

        public ContactService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(int RequesterId, int ContactId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == RequesterId);
            var contact = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == ContactId);
            if (user == null || contact == null)
            {
                return false;
            }
            user.Contacts.Add(contact);
            var updated = await _dbContext.SaveChangesAsync();
            return updated > 0 ? true : false;
        }

        public async Task<UserCard> GetUserAsync(int RequesterId, int ContactID)
        {
            throw new NotImplementedException();
        }

        public async Task<UserCard> GetUserAsync(int RequesterId, string ContactAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(int RequesterId, int ContactId)
        {
            throw new NotImplementedException();
        }
    }
}
