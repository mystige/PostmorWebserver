using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Contracts.Requests
{
    public class MessageFetchNewRequest
    {
        public string Token{ get; set; }
        public string LatestMessageId { get; set; }
    }
}
