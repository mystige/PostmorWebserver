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
        [Required]
        public DateTime ReceivedTime { get; set; }
        [Required]
        public string Type { get; set; }
        public string[] Message { get; set; }
        [Required]
        public int SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }
        [Required]
        public int RetrieverId { get; set; }
        [ForeignKey(nameof(RetrieverId))]
        public User Retriver { get; set; }
    }
}
