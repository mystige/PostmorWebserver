using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Data.Entities
{
    public class Letter
    {   [Key]
        public int Id { get; set; }
        [Required]
        public DateTime ReceivedTime { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public User Sender { get; set; }
        [Required]
        public User Retriver { get; set; }
    }
}
