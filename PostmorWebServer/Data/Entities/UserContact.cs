using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PostmorWebServer.Data.Entities
{
    public class UserContact
    {
        [Key]
        public int Id { get; set; } 
        public int User1Id { get; set; }
        [ForeignKey(nameof(User1Id))]
        public User User1 { get; set; }    
        public int User2Id { get; set; }
        [ForeignKey(nameof(User2Id))]
        public User User2 { get; set; }



    }
}
