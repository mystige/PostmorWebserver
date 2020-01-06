using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using PostmorWebServer.Data;
using PostmorWebServer.Data.Entities;
using PostmorWebServer.Models;

namespace PostmorWebServer.Services
{
    public class MessageService : IMessageService
    {
        private readonly DataContext _dbContext;

        public MessageService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Message>> FetchNewAsync(string token, int lastMsgId)
        {
            int requesterId = ExtractIdFromJwtToken(token);
            throw new NotImplementedException();
        }

        public async Task<int> SendAsync(string[] message, string type, string token, int reciverId)
        {
            int senderId = ExtractIdFromJwtToken(token);
            var newMsg = new Letter {
                Type = type,
                RetrieverId = reciverId,
                Retriver = await _dbContext.Users.FindAsync(reciverId),
                SenderId = senderId,
                Sender = await _dbContext.Users.FindAsync(senderId),
                ReceivedTime = DateTime.UtcNow.AddHours(48),
                Message = message                                 
            };
            await _dbContext.Letters.AddAsync(newMsg);
            await _dbContext.SaveChangesAsync();
            return newMsg.Id;

          
        }

        

        private int ExtractIdFromJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == "id").Value;
            return Convert.ToInt32(id);
        }
    }
}
