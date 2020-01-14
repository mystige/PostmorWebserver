using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Timestamp { get; set; }
        public string DeliveryTime { get; set; }
        public string[] Content { get; set; }
        public string Type { get; set; }
    }
}
