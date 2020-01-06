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
        public int ReceiverID { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string[] Content { get; set; }
        public string Type { get; set; }
    }
}
