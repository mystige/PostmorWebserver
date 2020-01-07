using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PostmorWebServer.Data;
using PostmorWebServer.Data.Entities;
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

        public async Task<AddOrRemoveContactResult> AddAsync(string token, int ContactId)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            if (requesterId == ContactId)
            {
                return new AddOrRemoveContactResult
                {
                    Success = false,
                    Error =  "Not possible to befriend yourself"
                };
            }
            var user = await _dbContext.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(x => x.Id == requesterId);
            var contact = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == ContactId);
            if (user == null || contact == null)
            {
                return new AddOrRemoveContactResult
                {
                    Success = false,
                    Error = "User does not exist"
                };
            }
            user.Contacts.Add(contact);
            var updated = await _dbContext.SaveChangesAsync();
            return new AddOrRemoveContactResult { Success = true };
        }

        public async Task<AddOrRemoveContactResult> RemoveAsync(string token, int ContactId)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            if (requesterId == ContactId)
            {
                return new AddOrRemoveContactResult
                {
                    Success = false,
                    Error = "Not possible to befriend yourself"
                };
            }
            var requester = await _dbContext.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(x => x.Id == requesterId);
            var contact = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == ContactId);

            if (requester.Contacts.Contains(contact))
            {
                var result = requester.Contacts.Remove(contact);
                return new AddOrRemoveContactResult { Success = true };
            }
            return new AddOrRemoveContactResult { Error = "This user is not your contact" };
        }

        public async Task<UserCard> FindUserByAddressAsync(string token, string ContactAddres)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            char[] Number = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ' ' };
            string addres = ContactAddres.TrimEnd(Number);
            var contact = await _dbContext.Users
                .SingleOrDefaultAsync(x => x.Address == addres);
            var user = await _dbContext.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(x => x.Id == requesterId);

            if (contact.ActiveUser)
            {
                return GenerateUserCard(user, contact);
            }
            return new UserCard { Error = "This user is inactive"};
        }

        public async Task<UserCard> FindUserByIdAsync(string token, int ContactId)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            var contact = await _dbContext.Users                
                .SingleOrDefaultAsync(x => x.Id == ContactId);
            var user = await _dbContext.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(x => x.Id == requesterId);

            if (contact.ActiveUser)
            {
                return GenerateUserCard(user, contact);
            }
            return new UserCard { Error = "This user is inactive" };
        }
        
        private int ExtractIdFromJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == "id").Value;
            return Convert.ToInt32(id);
        }

        private UserCard GenerateUserCard(User requester, User contact)
        {
            if (requester == null)
            {
                return new UserCard
                {
                    Success = false,
                    Error = "You were not found?"
                };
            }

            if (contact == null)
            {
                return new UserCard
                {
                    Success = false,
                    Error = "Address not found"
                };
            }
            var userCard = new UserCard
            {
                ContactId = contact.Id,
                Picture = contact.ProfilePic,
                Success = true,
                PublicKey = contact.PublicKey,
                ContactAddress = contact.Address + " " + contact.Streetnumber,
                ContactName = contact.Name
            };
            if (requester.Id != contact.Id && requester.Contacts.Contains(contact))
            {
                userCard.IsFriend = true;
            }
            else
            {
                userCard.IsFriend = false;
            }
            return userCard;
        }

    }
}
