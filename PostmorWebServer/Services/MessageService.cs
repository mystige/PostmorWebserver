using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            int requesterID = ExtractIdFromJwtToken(token);
            var letters = await _dbContext.Letters.Where(x => x.RetrieverId == requesterID && x.Id > lastMsgId).ToListAsync();
            List<Message> messages = new List<Message>();
            foreach (var letter in letters)
            {
                messages.Add(new Message 
                {
                    MessageId = letter.Id,
                    SenderId = letter.SenderId,
                    ReceiverId = letter.RetrieverId,
                    Content = letter.Message,
                    Type = letter.Type,
                    DeliveryTime = letter.ReceivedTime.ToString("dd-MM-yyyy HH:mm"),
                    Timestamp = letter.ReceivedTime.ToString("dd-MM-yyyy HH:mm")
                });
            }
            return messages;
        }
        
        public async Task<Tuple<int,string>> SendAsync(string[] message, string type, string token, int reciverId)
        {
            int senderId = ExtractIdFromJwtToken(token);
            var newMsg = new Letter {
                Type = type,
                RetrieverId = reciverId,
                Retriver = await _dbContext.Users.FindAsync(reciverId),
                SenderId = senderId,
                Sender = await _dbContext.Users.FindAsync(senderId),
                ReceivedTime = DateTime.UtcNow,
                Message = message                                 
            };
            await _dbContext.Letters.AddAsync(newMsg);
            await _dbContext.SaveChangesAsync();
            return new Tuple<int, string>(newMsg.Id, newMsg.ReceivedTime.ToString("dd-MM-yyyy HH:mm"));    
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
