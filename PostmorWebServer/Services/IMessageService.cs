using PostmorWebServer.Data.Entities;
using PostmorWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Services
{
    public interface IMessageService
    {
        Task<int> SendAsync(string[] Message, string Type, string SenderToken, int ReciverId);
        Task<List<Message>> FetchNewAsync(string SenderToken, int LastMsgId);
    }
}
