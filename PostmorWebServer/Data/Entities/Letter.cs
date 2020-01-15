using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Data.Entities
{
    public class Letter
    {   [Key]
        public int Id { get; set; }
        public DateTime ReceivedTime { get; set; }
        public string Type { get; set; }
        public string[] Message { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int RetrieverId { get; set; }
        public User Retriver { get; set; }
    }
}
