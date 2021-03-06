﻿using System;
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
            var contact = await _dbContext.Users
                .Where(u => u.Id == ContactId)
                .SingleOrDefaultAsync();
            if (contact == null)
            {
                return new AddOrRemoveContactResult
                {
                    Success = false,
                    Error = "User does not exist"
                };
            }

            var user = await _dbContext.Users
                .Include(u => u.Contacts)
                .Where(u => u.Id == requesterId)
                .SingleOrDefaultAsync();

            if (user == null)
                return new AddOrRemoveContactResult
                {
                    Success = false,
                    Error = "You does not exist"
                };

            if (user.Contacts.Any(x=> x.User1Id == user.Id && x.User2Id == contact.Id))
            {
                return new AddOrRemoveContactResult
                {
                    Success = false,
                    Error = "User already your friend"
                };
            }
            
            var test = new UserContact
            {
                User1 = user,
                User1Id = 1,
                User2 = contact,
                User2Id = ContactId
            };
            user.Contacts.Add(test);

            var updated = await _dbContext.SaveChangesAsync();
            
            return new AddOrRemoveContactResult { Success = updated > 0 };
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
            
            if(contact == null)
            {
                return new AddOrRemoveContactResult { Error = "This user does not exist", Success = false };
            }
            var relation = requester.Contacts.Find(x => x.User1Id == requester.Id && x.User2Id == contact.Id);
            if (relation!=null)
            {
                var result = requester.Contacts.Remove(relation);
                await _dbContext.SaveChangesAsync();
                return new AddOrRemoveContactResult { Success = true };
            }
            return new AddOrRemoveContactResult { Error = "This user is not your contact", Success = false };
        }

        public async Task<UserCard> FindUserByAddressAsync(string token, string ContactAddres)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            var address = ContactAddres.Split(' ');
            var contact = await _dbContext.Users
                .SingleOrDefaultAsync(x => x.Address == address[0]);
            if (contact == null)
            {
                return new UserCard
                {
                    Error = "This user does not exist",
                    Success = false
                };
            }
            var user = await _dbContext.Users
                .Include(x => x.Contacts)

                .SingleOrDefaultAsync(x => x.Id == requesterId);
            if(user == null)
            {
                return new UserCard
                {
                    Error = "You does not exist?",
                    Success = false
                };
            }
            if (contact.ActiveUser)
            {
                return GenerateUserCard(user, contact);
            }
            return new UserCard
            {
                Error = "This user is inactive",
                Success = false
            };
        }

        public async Task<UserCard> FindUserByIdAsync(string token, int ContactId)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            var contact = await _dbContext.Users                
                .SingleOrDefaultAsync(x => x.Id == ContactId);
            var user = await _dbContext.Users
                .Include(u => u.Contacts)
                .SingleOrDefaultAsync(x => x.Id == requesterId);
            if (contact == null)
            {
                return new UserCard
                {
                    Error = "This user does not exist",
                    Success = false
                };
            }

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
            var relation = requester.Contacts.Find(x => x.User1Id == requester.Id && x.User2Id == contact.Id);
            if (relation != null && requester.Id != contact.Id) 
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
